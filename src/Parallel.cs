using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace dns_netcore
{
    class RecursiveResolver : IRecursiveResolver
    {
        private IDNSClient dnsClient;
        private ConcurrentDictionary<string, IP4Addr> cache = new ConcurrentDictionary<string, IP4Addr>();

        public RecursiveResolver(IDNSClient client)
        {
            dnsClient = client;
        }

        /// <summary>
        /// Recursively resolves a domain name to an IPv4 address.
        /// If the domain is cached, it retrieves the stored result.
        /// Otherwise, it resolves the domain step by step.
        /// </summary>
        /// <param name="domain">The domain name to resolve.</param>
        /// <returns>A task that resolves to the IPv4 address of the given domain.</returns>
        public async Task<IP4Addr> ResolveRecursive(string domain)
        {
            if (cache.TryGetValue(domain, out IP4Addr cachedAddress))
            {
                try
                {
                    string reversed = await this.dnsClient.Reverse(cachedAddress);
                    if (domain == reversed)
                    {
                        return cachedAddress;
                    }

                }
                catch (DNSClientException ex)
                {
                    cache.TryRemove(domain, out _);
                }
            }
            string[] domains = domain.Split('.');
            string childDomain = "";
            IP4Addr childAdress;

            if (domains.Length == 1) 
            { 
                childDomain = domains[0];
                childAdress = dnsClient.GetRootServers()[0];
            }
            else
            {
                for (int i = 1; i < domains.Length; i++)
                {
                    if (i > 1)
                    {
                        childDomain += ".";
                    }
                    childDomain += domains[i];
                }
                childAdress = await ResolveRecursive(childDomain);
            }

            var t = dnsClient.Resolve(childAdress, domains[0]);
            var tResult = await t;
            cache[domain] = tResult;
            return tResult;
        }
    }
}
