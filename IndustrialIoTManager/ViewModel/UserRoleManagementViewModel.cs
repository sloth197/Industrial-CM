using System.Collections.ObjectModel;
using IndustrialIoTManager.Model;
using IndustrialIoTManager.ViewModel.Base;

namespace IndustrialIoTManager.ViewModel;

public sealed class UserRoleManagementViewModel : ViewModelBase
{
    public UserRoleManagementViewModel()
    {
        Users =
        [
            new UserAccount { UserName = "admin", Role = "Administrator" },
            new UserAccount { UserName = "operator", Role = "Operator" },
            new UserAccount { UserName = "viewer", Role = "Viewer" }
        ];
    }

    public string Title => "사용자 및 권한 관리";

    public ObservableCollection<UserAccount> Users { get; }
}
