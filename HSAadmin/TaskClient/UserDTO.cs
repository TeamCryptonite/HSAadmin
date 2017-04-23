using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace TaskClient
{
    public class UserDTO
    {
        public UserDTO(string UserId, string FName, string LName, bool isAdmin, bool activeStatus)
        {
            this.UserGuid = UserId;
            this.GivenName = FName;
            this.Surname = LName;
            this.IsEmployee = isAdmin;
            this.IsActiveUser = activeStatus;
        }
        public string UserGuid { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public bool? IsEmployee { get; set; }
        public bool? IsActiveUser { get; set; }
    }

    public class Users : ObservableCollection<UserDTO>
    {
        public Users()
        {
            Add(new UserDTO("1234", "Aaron", "Qualls", true, true));
            Add(new UserDTO("1231", "Seth", "VonSeggern", true, true));
            Add(new UserDTO("1232", "Pearse", "Hutson", true, true));
            Add(new UserDTO("1233", "Devun", "Schmutzler", true, true));
            Add(new UserDTO("1235", "Steven", "Snow", true, true));
            Add(new UserDTO("1234", "Colin", "Iglehart", true, true));
        }
    }
}
