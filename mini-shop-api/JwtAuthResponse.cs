namespace mini_shop_api
{
    [Serializable]

    public class JwtAuthResponse
    {
        public string Token { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Id { get; set; }
        public int Expires_in { get; set; }
        public string Role { get; set; }    
    }
}
