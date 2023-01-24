namespace FileAPI
{
    public class MailRequest
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> ListOfMailAddresses { get; set; }
    }
}
