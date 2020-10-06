namespace datingApp.api.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 30; 
        public int PageNumber { get; set; } = 1; 
        private int _pageSize = 10;
    
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public int UserId { get; set; }
        public string CurrentUsername {set; get;}
        public string Gender {set;get;}
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 80;
        public string OrderBy { get; set; }
        
        
    }
}
        
    