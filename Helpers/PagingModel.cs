namespace XTL.Helpers
{
    public class PagingMedel
    {
        public int currentpage {set;get;}
        public int countpages {set; get;}
        public Func<int?, string> generateUrl {get; set;}
    }
}