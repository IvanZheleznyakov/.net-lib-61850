namespace lib61850net
{
        public enum IsoTpktState
        {
            TPKT_RECEIVE_START,
            TPKT_RECEIVE_ERROR,
            TPKT_RECEIVE_RES,
            TPKT_RECEIVE_LEN1,
            TPKT_RECEIVE_LEN2,
            TPKT_RECEIVE_DATA_COPY
        }
}
