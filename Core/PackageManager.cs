using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using CosmosHttp.Client;
using System;
using System.Net;

namespace NovaOS.Core
{
    public class PackageManager
    {
        public static byte[] DownloadRaw(string url)
        {
            if (url.StartsWith("https://"))
            {
                throw new WebException("HTTPS is not supported, use http://");
            }

            string path = ExtractPath(url);
            string domain = ExtractDomain(url);

            var dns = new DnsClient();

            dns.Connect(DNSConfig.DNSNameservers[0]);
            dns.SendAsk(domain);
            Address addr = dns.Receive();
            dns.Close();

            HttpRequest req = new();
            req.IP = addr.ToString();
            req.Domain = domain;
            req.Path = path;
            req.Method = "GET";
            req.Send();

            return req.Response.GetStream();
        }

        public static string Download(string url)
        {
            if (url.StartsWith("https://"))
            {
                throw new WebException("HTTPS is not supported, use http://");
            }

            string path = ExtractPath(url);
            string domain = ExtractDomain(url);

            var dns = new DnsClient();

            dns.Connect(DNSConfig.DNSNameservers[0]);
            dns.SendAsk(domain);
            Address addr = dns.Receive();
            dns.Close();

            HttpRequest req = new();
            req.IP = addr.ToString();
            req.Domain = domain;
            req.Path = path;
            req.Method = "GET";
            req.Send();

            return req.Response.Content;
        }

        private static string ExtractDomain(string url)
        {
            int start;

            if (url.Contains("://"))
            {
                start = url.IndexOf("://") + 3;
            }
            else
            {
                start = 0;
            }

            int end = url.IndexOf("/", start);
            if (end == -1)
            {
                end = url.Length;
            }

            return url[start..end];
        }

        private static string ExtractPath(string url)
        {
            int start;

            if (url.Contains("://"))
            {
                start = url.IndexOf("://") + 3;
            }
            else
            {
                start = 0;
            }

            int idx = url.IndexOf("/", start);
            if (idx != -1)
            {
                return url.Substring(idx);
            }
            else
            {
                return "/";
            }
        }
    }
}
