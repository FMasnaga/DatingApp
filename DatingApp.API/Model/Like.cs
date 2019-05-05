namespace DatingApp.API.Model
{
    public class Like
    {
        public int HisId {get;set;}
        
        public int HerId {get;set;}
        
        public User His {get;set;}
        
        public User Her {get;set;}
    }
}