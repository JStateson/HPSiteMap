using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HPSiteMap
{

    internal class cItem
    {
        public List<string> EachUrl;
        public void Init()
        {
            EachUrl = new List<string>();
        }
        public void Add(string s)
        {
            EachUrl.Add(s);
        }
    }
    internal class Program
    {
        static int ColumnsWanted = 3;
        static int NumItems = 0;
        static int nRows = 0;
        static List<cItem> ListUrls = new List<cItem>();
        private static string FormUrl(string strUrl, string strIn)
        {
            if (strIn == "") strIn = strUrl;
            return "<a href=\"" + strUrl + "\" target=\"_blank\">" + strIn + "</a>";
        }

        private static string dup_n(string s, int n)
        {
            string t = "";
            for (int i = 0; i < n;i++)
            {
                t += s;
            }
            return t;
        }


        static string ExtractItem(string s, ref string sAsk)
        {
            int i = s.IndexOf("/bd-p/");
            int n = i+6;
            if (i < 0)
            {
                i = s.IndexOf("/bg-p/");
            }
            if (i < 0)
            {
                i = s.IndexOf("/tkb-p/");
                n = i+7;
            }
            if (i < 0) return "";
            int j = s.IndexOf("/t5/");
            if (j < 0) return "";
            sAsk = s.Substring(n);
            string t = s.Substring(j + 4, i - (j + 4));
            s = t.Replace("-and-", "");
            t = s.Replace("-or-", "");
            s = t.Replace("-", "");
            return s;
        }

        static int MakeDivisible(int n, int m)
        {
            int remainder = n % m;
            if (remainder == 0) return n;
            return n + (m - remainder);
        }

        static void Main(string[] args)
        {
            cItem ci = new cItem();
            ci.Init();
            List<string> ul = new List<string>();
            int i, j, n;
            string aLine;
            using (StreamReader sr = new StreamReader("../../SiteMapRaw.txt"))
            {
                // Read the stream to a string, and write the string to the console.
                string s;

                while ((s = sr.ReadLine()) != null)
                {
                    if(s == "")
                    {
                        ListUrls.Add(ci);
                        ci = new cItem();
                        ci.Init();
                        continue;
                    }
                    ci.Add(s);
                }
            }
            string LineOut = "<table border='1'>";
            foreach ( cItem cI in  ListUrls )
            {
                if (cI.EachUrl.Count == 0) continue;
                string sRawN = cI.EachUrl[0];
                i = sRawN.IndexOf("/t5/");
                j = sRawN.IndexOf("/ct-p/");
                Debug.Assert(i>0 && j>0);
                string sN = sRawN.Substring(i + 4, j - (i + 4));
                j = cI.EachUrl.Count;
                for(i = 1; i < j; i++)
                {
                    NumItems++;
                    LineOut += "<tr>";
                    string s = cI.EachUrl[i];
                    string sAsk = "";
                    string sP = ExtractItem(s, ref sAsk);
                    string UrlF = FormUrl(s, sP);
                    LineOut += "<td>" + UrlF + "</td>";
                    string UrlA = FormUrl("https://h30434.www3.hp.com/t5/forums/postpage/board-id/" + sAsk, "Start a conversation");
                    LineOut += "<td>" + UrlA + "</td>";
                    ul.Add(UrlA);
                    LineOut += "</tr>";
                }
            }
            LineOut += "</table>";
            File.WriteAllText("../../SiteMap.html",LineOut);
            n = MakeDivisible(NumItems, ColumnsWanted);
            nRows = n / ColumnsWanted;

            if(n != NumItems)
            {
                int d = n - NumItems;
                for(i = 0; i < d; i++)
                {
                    ul.Add("");
                }
            }

            LineOut = "";
            LineOut += "# HP SiteMap of Community 'Ask a question'\n";
            LineOut += dup_n("|", ColumnsWanted) + "|\n";
            LineOut += dup_n("| ------------- ", ColumnsWanted) + "|\n";
            n = 0;
            for(i = 0; i < nRows; i++)
            {
                for (j = 0; j < ColumnsWanted; j++)
                {
                    aLine = "|" + ul[i + j * nRows];
                    LineOut += aLine;
                }
                LineOut += "|\n";
            }
            File.WriteAllText("../../README.md", LineOut);
        }
    }
}
