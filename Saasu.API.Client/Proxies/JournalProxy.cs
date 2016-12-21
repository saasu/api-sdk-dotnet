using Saasu.API.Client.Framework;
using Saasu.API.Core;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Journals;
using System.Net.Http;

namespace Saasu.API.Client.Proxies
{
    public class JournalProxy : BaseProxy
    {
        public string _requestPrefix = ResourceNames.Journal;

        public JournalProxy()
            : base()
        {
            ContentType = RequestContentType.ApplicationJson;
        }

        public JournalProxy(string bearerToken) : base(bearerToken) { }

        public JournalProxy(string baseUri, string wsAccessKey, int fileUid)
            : base(baseUri, wsAccessKey, fileUid)
        {
        }

        public override string RequestPrefix
        {
            get { return _requestPrefix; }
        }

        public ProxyResponse<JournalTransactionSummaryResponse> GetJournal(int journalId)
        {
            OperationMethod = HttpMethod.Get;
            _requestPrefix = ResourceNames.Journal;
            var uri = base.GetRequestUri(journalId.ToString());
            return base.GetResponse<JournalTransactionSummaryResponse> (uri);
        }

        public ProxyResponse<UpdateJournalResult> UpdateJournal(int journalId, JournalDetail journalDetail)
        {
            OperationMethod = HttpMethod.Put;
            _requestPrefix = ResourceNames.Journal;
            var uri = base.GetRequestUri(journalId.ToString());
            return base.GetResponse<JournalDetail, UpdateJournalResult>(uri, journalDetail);
        }

        public ProxyResponse<BaseResponseModel> DeleteJournal(int journalId)
        {
            _requestPrefix = ResourceNames.Journal;
            OperationMethod = HttpMethod.Delete;
            var uri = base.GetRequestUri(journalId.ToString());
            return base.GetResponse<BaseResponseModel>(uri);
        }

        public ProxyResponse<InsertJournalResult> InsertJournal(JournalDetail journalDetail)
        {
            _requestPrefix = ResourceNames.Journal;
            OperationMethod = HttpMethod.Post;
            var uri = base.GetRequestUri(null);
            return base.GetResponse<JournalDetail, InsertJournalResult>(uri, journalDetail);
        }
    }
}
