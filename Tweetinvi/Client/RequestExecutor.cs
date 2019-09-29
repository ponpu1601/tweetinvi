﻿using Tweetinvi.Client.Requesters;

namespace Tweetinvi.Client
{
    public interface IInternalRequestExecutor : IRequestExecutor
    {
        void Initialize(TwitterClient client);
    }

    public class RequestExecutor : IInternalRequestExecutor
    {
        private readonly IInternalAccountRequester _accountRequester;
        private readonly IInternalTweetsRequester _tweetsRequester;
        private readonly IInternalUploadRequester _uploadRequester;
        private readonly IInternalUsersRequester _usersRequester;

        public RequestExecutor()
        {
        }
        
        public RequestExecutor(
            IInternalAccountRequester accountRequester,
            IInternalTweetsRequester tweetsRequester,
            IInternalUploadRequester uploadRequester,
            IInternalUsersRequester usersRequester)
        {
            _accountRequester = accountRequester;
            _tweetsRequester = tweetsRequester;
            _uploadRequester = uploadRequester;
            _usersRequester = usersRequester;
        }

        public void Initialize(TwitterClient client)
        {
            _accountRequester.Initialize(client);
            _tweetsRequester.Initialize(client);
            _uploadRequester.Initialize(client);
            _usersRequester.Initialize(client);
        }

        public IAccountRequester Account => _accountRequester;
        public ITweetsRequester Tweets => _tweetsRequester;
        public IUploadRequester Upload => _uploadRequester;
        public IUsersRequester Users => _usersRequester;
    }
}
