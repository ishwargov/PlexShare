using PlexShareCloud;
using System.Text;

namespace PlexShareTests.CloudTests
{

    public class RestApiTest : IDisposable
    {
        private const string SubmissionUrl = @"http://localhost:7213/api/submission";
        private const string SessionUrl = @"http://localhost:7213/api/session";
        private FileDownloadApi _downloadClient;
        private FileUploadApi _uploadClient;

        public RestApiTest()
        {
            _downloadClient = new(SessionUrl, SubmissionUrl);
            _uploadClient = new(SessionUrl, SubmissionUrl);
        }
        ~RestApiTest()
        {
            Dispose();
        }
        /// <summary>
        /// Cleans up the test leftovers.
        /// </summary>
        public void Dispose()
        {
            _downloadClient.DeleteAllFilesAsync().Wait();
            _downloadClient.DeleteAllSessionsAsync().Wait();
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Tests creating and getting an entity.
        /// </summary>
        [Fact]
        public async Task TestPostAndGetSubmission()
        {
            // Create an entity.
            //Logger.LogMessage("Create an entity.");
            byte[] newPdf = Encoding.ASCII.GetBytes("author");
            SubmissionEntity? postEntity = await _uploadClient.PostSubmissionAsync("sessionId", "userName", newPdf);

            // Get the entity.
            //Logger.LogMessage("Get the entity.");

            IReadOnlyList<SubmissionEntity>? getEntity = await _downloadClient.GetFilesByUserAsync("userName");

            // Validate.
            //Logger.LogMessage("Validate.");
            Assert.Equal(1, getEntity?.Count);
            for(int i=0;i<getEntity?.Count;i++){
                Assert.Equal(postEntity?.SessionId, getEntity[i].SessionId);
            }

            //Assert.Equal(postEntity?.Name, getEntity?.Name);
        }

        [Fact]
        public async Task TestCreateAndGetSession()
        {
            
            SessionEntity? postEntity = await _uploadClient.PostSessionAsync("sessionId", "hostuserName");

            // Get the entity.
            //Logger.LogMessage("Get the entity.");

            IReadOnlyList<SessionEntity>? getEntity = await _downloadClient.GetSessionsByUserAsync("hostuserName");

            // Validate.
            //Logger.LogMessage("Validate.");
            Assert.Equal(1, getEntity?.Count);
            for (int i = 0; i < getEntity?.Count; i++)
            {
                Assert.Equal(postEntity?.SessionId, getEntity[i].SessionId);
            }
        }

        [Fact]
        public async Task TestUpdateSubmission()
        {
            // Create an entity.
            //Logger.LogMessage("Create an entity.");
            byte[] newPdf = Encoding.ASCII.GetBytes("author");
            byte[] changedPdf = Encoding.ASCII.GetBytes("writer");

            SubmissionEntity? postEntity = await _uploadClient.PostSubmissionAsync("sessionId", "userName", newPdf);
            SubmissionEntity? updateEntity = await _uploadClient.PutSubmissionAsync("sessionId", "userName", changedPdf);

            // Get the entity.
            //Logger.LogMessage("Get the entity.");

            IReadOnlyList<SubmissionEntity>? getEntity = await _downloadClient.GetFilesByUserAsync("userName");

            // Validate.
            //Logger.LogMessage("Validate.");
            Assert.Equal(1, getEntity?.Count);
            for (int i = 0; i < getEntity?.Count; i++)
            {
                //Assert.Equal(postEntity?.SessionId, getEntity[i].SessionId);
                if (getEntity[i].SessionId == postEntity.SessionId)
                {
                    Assert.Equal(changedPdf, getEntity[i].Pdf);
                }
            }
        }

        [Fact]
        public async Task TestGetFilesByUserInEmpty()
        {

            IReadOnlyList<SubmissionEntity>? getEntity = await _downloadClient.GetFilesByUserAsync("hostuserName");

            // Validate.
            //Logger.LogMessage("Validate.");
            Assert.Equal(0, getEntity?.Count); //0 indicates empty case. 
            
        }

        [Fact]
        public async Task TestGetFilesBySessionInEmpty()
        {

            IReadOnlyList<SubmissionEntity>? getEntity = await _downloadClient.GetFilesBySessionIdAsync("hostuserName");

            // Validate.
            //Logger.LogMessage("Validate.");
            Assert.Equal(0, getEntity?.Count); //0 indicates empty case. 

        }

        [Fact]
        public async Task TestGetSessionByUserInEmpty()
        {

            IReadOnlyList<SessionEntity>? getEntity = await _downloadClient.GetSessionsByUserAsync("hostuserName");

            // Validate.
            //Logger.LogMessage("Validate.");
            Assert.Equal(0, getEntity?.Count); //0 indicates empty case. 

        }
    }
}
