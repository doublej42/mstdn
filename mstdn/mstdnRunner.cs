using jsonDb;
using Mastonet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace mstdn
{
    internal class MstdnRunner
    {
        private readonly ILogger<MstdnRunner> log;
        private readonly IConfiguration config;
        private readonly JsonDb<JsonDbStruct> jsonDb;

        public MstdnRunner(ILogger<MstdnRunner> log, IConfiguration config, JsonDb<JsonDbStruct> jsonDb)
        {
            this.log = log;
            this.config = config;
            this.jsonDb = jsonDb;
        }
        public async Task RunAsync()
        {
            log.LogTrace("Starting RunAsync");
            var instance = config.GetValue<String>("instance");// "mstdn.ca";
            if (instance == null)
            {
                throw new Exception("Instance not found in config, this is your mastedon domain");
            }
            if (jsonDb.Data.AccessToken  == null)
            {
                var authClient = new AuthenticationClient(instance);
                var appRegistration = await authClient.CreateApp("Test App Mastonet", Scope.Read | Scope.Write | Scope.Follow);

                var url = authClient.OAuthUrl();

                Console.WriteLine("Please open the following url in your browser:");
                Console.WriteLine(url);
                string authCode = "";
                Console.WriteLine("Paste the code in here");
                while (string.IsNullOrWhiteSpace(authCode))
                {
#pragma warning disable CS8601 // Possible null reference assignment.
                    authCode = Console.ReadLine();
#pragma warning restore CS8601 // Possible null reference assignment.
                }
                var auth = await authClient.ConnectWithCode(authCode);
                jsonDb.Data.AccessToken = auth.AccessToken;
                jsonDb.Save();
            }

            var client = new MastodonClient(instance, jsonDb.Data.AccessToken);
            var newPost = await client.PublishStatus($"The time here is : {DateTime.Now.ToString()}");
            log.LogDebug("Post", newPost);
        }
    }
}
