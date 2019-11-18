﻿using System.Threading.Tasks;
using Tweetinvi.Client.Requesters;
using Tweetinvi.Core.Client.Validators;
using Tweetinvi.Core.Factories;
using Tweetinvi.Core.Iterators;
using Tweetinvi.Core.Web;
using Tweetinvi.Iterators;
using Tweetinvi.Models;
using Tweetinvi.Models.DTO;
using Tweetinvi.Models.DTO.QueryDTO;
using Tweetinvi.Parameters;

namespace Tweetinvi.Client
{
    public class TweetsClient : ITweetsClient
    {
        private readonly TwitterClient _client;
        private readonly ITweetsRequester _tweetsRequester;
        private readonly ITweetFactory _tweetFactory;

        public TweetsClient(TwitterClient client)
        {
            _client = client;
            _tweetsRequester = client.RequestExecutor.Tweets;
            _tweetFactory = TweetinviContainer.Resolve<ITweetFactory>();
        }
        
        public ITweetsClientParametersValidator ParametersValidator => _client.ParametersValidator;

        // Tweets

        public Task<ITweet> GetTweet(long? tweetId)
        {
            return GetTweet(new GetTweetParameters(tweetId));
        }

        public async Task<ITweet> GetTweet(IGetTweetParameters parameters)
        {
            var twitterResult = await _tweetsRequester.GetTweet(parameters).ConfigureAwait(false);
            return twitterResult?.Result;
        }

        public Task<ITweet[]> GetTweets(long[] tweetIds)
        {
            return GetTweets(new GetTweetsParameters(tweetIds));
        }
        
        public Task<ITweet[]> GetTweets(long?[] tweetIds)
        {
            return GetTweets(new GetTweetsParameters(tweetIds));
        }

        public Task<ITweet[]> GetTweets(ITweetIdentifier[] tweets)
        {
            return GetTweets(new GetTweetsParameters(tweets));
        }

        public async Task<ITweet[]> GetTweets(IGetTweetsParameters parameters)
        {
            var requestResult = await _tweetsRequester.GetTweets(parameters).ConfigureAwait(false);
            return requestResult?.Result;
        }

        // Tweets - Publish

        public Task<ITweet> PublishTweet(string text)
        {
            return PublishTweet(new PublishTweetParameters(text));
        }

        public async Task<ITweet> PublishTweet(IPublishTweetParameters parameters)
        {
            var requestResult = await _tweetsRequester.PublishTweet(parameters).ConfigureAwait(false);
            return requestResult?.Result;
        }

        // Tweets - Destroy

        public Task<bool> DestroyTweet(long? tweetId)
        {
            return DestroyTweet(new DestroyTweetParameters(tweetId));
        }

        public Task<bool> DestroyTweet(ITweetIdentifier tweet)
        {
            return DestroyTweet(new DestroyTweetParameters(tweet));
        }

        public Task<bool> DestroyTweet(ITweet tweet)
        {
            return DestroyTweet(tweet.TweetDTO);
        }
        
        public async Task<bool> DestroyTweet(ITweetDTO tweet)
        {
            var tweetDestroyed = await DestroyTweet(new DestroyTweetParameters(tweet)).ConfigureAwait(false);
            
            if (tweetDestroyed)
            {
                tweet.IsTweetDestroyed = true;
            }

            return tweetDestroyed;
        }

        public async Task<bool> DestroyTweet(IDestroyTweetParameters parameters)
        {
            var twitterResult = await _tweetsRequester.DestroyTweet(parameters).ConfigureAwait(false);
            return twitterResult.Response.IsSuccessStatusCode;
        }

        // Retweets

        public Task<ITweet[]> GetRetweets(long? tweetId)
        {
            return GetRetweets(new GetRetweetsParameters(tweetId));
        }

        public Task<ITweet[]> GetRetweets(ITweetIdentifier tweet)
        {
            return GetRetweets(new GetRetweetsParameters(tweet));
        }

        public async Task<ITweet[]> GetRetweets(IGetRetweetsParameters parameters)
        {
            var requestResult = await _tweetsRequester.GetRetweets(parameters).ConfigureAwait(false);
            return requestResult?.Result;
        }

        public Task<ITweet> PublishRetweet(long? tweetId)
        {
            return PublishRetweet(new PublishRetweetParameters(tweetId));
        }

        public Task<ITweet> PublishRetweet(ITweetIdentifier tweet)
        {
            return PublishRetweet(new PublishRetweetParameters(tweet));
        }
        
        public async Task<ITweet> PublishRetweet(IPublishRetweetParameters parameters)
        {
            var requestResult = await _tweetsRequester.PublishRetweet(parameters).ConfigureAwait(false);
            return requestResult?.Result;
        }

        public Task<bool> DestroyRetweet(long? retweetId)
        {
            return DestroyRetweet(new DestroyRetweetParameters(retweetId));
        }
        
        public Task<bool> DestroyRetweet(ITweetIdentifier retweet)
        {
            return DestroyRetweet(new DestroyRetweetParameters(retweet));
        }

        public async Task<bool> DestroyRetweet(IDestroyRetweetParameters parameters)
        {
            var requestResult = await _tweetsRequester.DestroyRetweet(parameters).ConfigureAwait(false);
            return requestResult?.Response?.IsSuccessStatusCode == true;
        }

        public ITwitterIterator<long> GetRetweeterIdsIterator(long? tweetId)
        {
            return GetRetweeterIdsIterator(new GetRetweeterIdsParameters(tweetId));
        }

        public ITwitterIterator<long> GetRetweeterIdsIterator(ITweetIdentifier tweet)
        {
            return GetRetweeterIdsIterator(new GetRetweeterIdsParameters(tweet));
        }

        public ITwitterIterator<long> GetRetweeterIdsIterator(IGetRetweeterIdsParameters parameters)
        {
            var twitterResultIterator = _tweetsRequester.GetRetweeterIds(parameters);
            return new TwitterIteratorProxy<ITwitterResult<IIdsCursorQueryResultDTO>, long>(twitterResultIterator, dto => dto.DataTransferObject.Ids);
        }

        #region Favourite Tweets

        public ITwitterIterator<ITweet, long?> GetFavoriteTweets(long? userId)
        {
            return GetFavoriteTweets(new GetFavoriteTweetsParameters(userId));
        }

        public ITwitterIterator<ITweet, long?> GetFavoriteTweets(string username)
        {
            return GetFavoriteTweets(new GetFavoriteTweetsParameters(username));
        }

        public ITwitterIterator<ITweet, long?> GetFavoriteTweets(IUserIdentifier user)
        {
            return GetFavoriteTweets(new GetFavoriteTweetsParameters(user));
        }

        public ITwitterIterator<ITweet, long?> GetFavoriteTweets(IGetFavoriteTweetsParameters parameters)
        {
            var tweetMode = _client.Config.TweetMode;

            var favoriteTweetsIterator = _tweetsRequester.GetFavoriteTweets(parameters);
            return new TwitterIteratorProxy<ITwitterResult<ITweetDTO[]>, ITweet, long?>(favoriteTweetsIterator,
                twitterResult =>
                {
                    return _tweetFactory.GenerateTweetsFromDTO(twitterResult.DataTransferObject, tweetMode, _client);
                });
        }

        #endregion
    }
}