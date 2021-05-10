using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;

public interface IRedisBase
{
    #region [ PUBLISH - SUBCRIBE ]

    bool PSUBSCRIBE(string channel);
    bool PUBLISH(string channel, long value);
    bool PUBLISH(string channel, byte[] vals);
    bool PUBLISH(string channel, string value);

    #endregion

    #region [ EXIST ]

    bool HEXISTS(string key, string field);

    #endregion

    #region [ GET ]

    string[] KEYS(string pattern = "*");

    string GET(string key);

    Bitmap GET_BITMAP(string key);

    Stream GET_STREAM(string key);

    byte[] GET_BUFFER(string key);

    Bitmap HGET_BITMAP(long key, int field);
    Bitmap HGET_BITMAP(string key, string field);
       

    string HGET(string key, string field);

    byte[] HGET_BUFFER(string key, string field);

    int[] HKEYS(long key);

    string[] HKEYS(string key);

    #endregion

    #region [ SET ]

    bool HSET(long key, int field, byte[] value);
    bool HSET(string key, string field, byte[] value);
    bool HSET(string key, string field, string value);
    bool HMSET(string key, IDictionary<string, string> fields);
    bool HMSET(string key, IDictionary<string, byte[]> fields);

    #endregion

    #region [ SEND TO COMMAND ]

    string SendToCommand(string channel, COMMANDS cmd, string data);

    #endregion

    #region [ REPLY DOCUMENT STATUS ]

    bool ReplyRequest(string requestId, string cmd, int ok = 1, long docId = 0, int page = 0, string tag = "", string file = "", string err = "");
    bool ReplyRequest(string requestId, string cmd, int ok, long docId, string tag, string err);
    bool ReplyRequest(string requestId, string cmd, int ok, long docId, string tag);
    bool ReplyRequest(string requestId, string cmd, int ok, long docId);

    bool ReplyRequest(string requestId, string cmd, int ok, string tag, string input);
    bool ReplyRequest(string requestId, string cmd, int ok, string tag, string input, string output);

    #endregion
}
