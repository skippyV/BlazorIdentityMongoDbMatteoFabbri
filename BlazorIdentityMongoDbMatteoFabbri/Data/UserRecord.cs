namespace BlazorIdentityMongoDbMatteoFabbri.Data
{
    public class UserRecord
    {
		private bool isSelected;

        public bool IsSelected
		{
			get { return isSelected; }
			set { isSelected = value; }
		}
		private ApplicationUser user;

        public ApplicationUser GetUser()
        { return user; }

        public void SetUser(ApplicationUser value)
        { user = value; }

        public UserRecord(bool isSelected)
        {
            IsSelected = isSelected;
        }
    }
}
