using GreatUma.Models;

namespace GreatUma.Infrastructures
{
    public class LoginConfigRepository : BaseRepository<LoginConfig>
    {
        public LoginConfigRepository() : base(@"login_config.xml")
        {
        }
    }
}
