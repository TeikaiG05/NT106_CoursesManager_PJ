using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace NT106_BT2
{
    /// <summary>
    /// ============================================================================
    /// GroupMembersHelper.cs - Helper class để lấy danh sách thành viên lớp
    /// ============================================================================
    /// 
    /// CHỨC NĂNG CHÍNH:
    /// Gửi yêu cầu GET_GROUP_MEMBERS tới server
    /// Chờ response với timeout
    /// Hiển thị dialog danh sách thành viên
    /// 
    /// CÔNG VIỆC CỤ THỂ:
    /// 
    /// 1. RequestMembersAsync(roomCode, timeoutMs = 10000) → Task<IEnumerable<MemberDto>>
    ///    - Tạo requestId unique (GUID)
    ///    - Tạo TaskCompletionSource để chờ response
    ///    - Subscribe vào TcpHelper.OnMessageReceived
    ///    - Parse JSON nhận về
    ///    - Nếu type = "GROUP_MEMBERS" và requestId match:
    ///      * Unsubscribe handler
    ///      * Complete task với danh sách members
    ///    - Timeout nếu chờ > 10 giây
    ///    - Throw TimeoutException nếu timeout
    /// 
    /// 2. ShowMembersDialogAsync(roomCode, owner = null) → Task
    ///    - Gọi RequestMembersAsync() để lấy danh sách
    ///    - Nếu lỗi: hiển thị error MessageBox
    ///    - Nếu OK: mở MembersForm dialog với danh sách
    ///    - Sử dụng BeginInvoke để ensure UI thread
    /// 
    /// NESTED CLASSES:
    /// MemberDto
    ///    - email: string
    ///    - fullName: string
    ///    - role: string (Teacher, Student, ...)
    /// 
    /// GetGroupMembersReq
    ///    - type = "GET_GROUP_MEMBERS"
    ///    - roomCode: string
    ///    - requestId: string (unique ID để match response)
    /// 
    /// GroupMembersRes
    ///    - type: string
    ///    - members: MemberDto[]
    ///    - requestId: string (match request)
    /// 
    /// DỊCH VỤ LIÊN KẾT:
    /// - TcpHelper: Gửi/nhận request
    /// - MembersForm: Dialog hiển thị danh sách
    /// 
    /// PATTERN DÙNG:
    /// ```csharp
    /// await GroupMembersHelper.ShowMembersDialogAsync(roomCode);
    /// ```
    /// 
    /// ============================================================================
    /// </summary>
    public class MemberDto
    {
        public string email { get; set; }
        public string fullName { get; set; }
        public string role { get; set; }
    }

    public class GetGroupMembersReq
    {
        public string type { get; set; } = "GET_GROUP_MEMBERS";
        public string roomCode { get; set; }
        public string requestId { get; set; }
    }

    public class GroupMembersRes
    {
        public string type { get; set; }
        public MemberDto[] members { get; set; }
        public string requestId { get; set; }
    }

    public static class GroupMembersHelper
    {
        public static async Task<IEnumerable<MemberDto>> RequestMembersAsync(string roomCode, int timeoutMs = 10000)
        {
            if (string.IsNullOrWhiteSpace(roomCode)) throw new ArgumentException(nameof(roomCode));

            var tcs = new TaskCompletionSource<IEnumerable<MemberDto>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var cts = new CancellationTokenSource();

            string requestId = Guid.NewGuid().ToString("N");

            Action<string> handler = null;
            handler = (line) =>
            {
                try
                {
                    Debug.WriteLine("[DEBUG] Incoming: " + line);
                    var probe = JsonConvert.DeserializeObject<dynamic>(line);
                    string t = (string)probe?.type ?? "";
                    if (!string.Equals(t, "GROUP_MEMBERS", StringComparison.OrdinalIgnoreCase))
                        return;

                    string respId = (string)probe.requestId;
                    if (!string.IsNullOrEmpty(respId) && !string.Equals(respId, requestId, StringComparison.OrdinalIgnoreCase))
                        return;

                    var res = JsonConvert.DeserializeObject<GroupMembersRes>(line);
                    TcpHelper.OnMessageReceived -= handler;
                    cts.Cancel();
                    tcs.TrySetResult(res?.members ?? new MemberDto[0]);
                }
                catch (Exception ex)
                {
                    TcpHelper.OnMessageReceived -= handler;
                    cts.Cancel();
                    tcs.TrySetException(ex);
                }
            };

            TcpHelper.OnMessageReceived += handler;

            var req = new GetGroupMembersReq { roomCode = roomCode, requestId = requestId };
            string jsonReq = JsonConvert.SerializeObject(req);
            Debug.WriteLine("[DEBUG] Sending request: " + jsonReq);

            await TcpHelper.SendLineAsync(jsonReq);

            var delayTask = Task.Delay(timeoutMs, cts.Token).ContinueWith(_ => { }, TaskScheduler.Default);
            var completed = await Task.WhenAny(tcs.Task, delayTask);

            if (completed != tcs.Task)
            {
                try { TcpHelper.OnMessageReceived -= handler; } catch { }
                throw new TimeoutException("Timeout waiting for GROUP_MEMBERS response.");
            }

            return await tcs.Task;
        }

        public static async Task ShowMembersDialogAsync(string roomCode, IWin32Window owner = null)
        {
            IEnumerable<MemberDto> members;
            try
            {
                members = await RequestMembersAsync(roomCode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner ?? Application.OpenForms[0], "Lỗi khi lấy members: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Control uiOwner = owner as Control ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            if (uiOwner != null && !uiOwner.IsDisposed)
            {
                uiOwner.BeginInvoke(new Action(() =>
                {
                    using (var f = new MembersForm(members, roomCode))
                        f.ShowDialog(uiOwner);
                }));
            }
            else
            {
                using (var f = new MembersForm(members, roomCode))
                    f.ShowDialog();
            }
        }
    }
}