namespace Saasu.API.Core.Models.Users
{
    public class UserCredential : BaseModel
    {
        /// <summary>
        /// The username. For example, someone@emailhost.com.
        /// </summary>
        public string Username { get; set; }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }
}
