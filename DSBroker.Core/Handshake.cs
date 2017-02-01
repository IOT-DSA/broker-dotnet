using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSBroker
{
    public class Handshake
    {
        private static readonly Random _random = new Random();

        private readonly Broker _broker;

        public Handshake(Broker broker)
        {
            _broker = broker;
        }

        /// <summary>
        /// Handles an incoming HTTP post request to /conn.
        /// </summary>
        /// <param name="postedData">JSON data that contains information about the connecting client.</param>
        /// <param name="dsId">DSLink identifier for connecting client.</param>
        /// <param name="token">Optional token for broker authentication.</param>
        /// <returns>Constructed client object from provided data.</returns>
        public Client HandleHandshake(string postedData, string dsId, string token = null)
        {
            var jsonIn = JObject.Parse(postedData);
            var client = new Client();
            client.Name = dsIdToName(dsId);
            client.TempKey = new KeyPair();

            if (string.IsNullOrEmpty(dsId))
            {
                throw new Exception("dsId must not be null or empty.");
            }
            else
            {
                if (!ValidateDsId(dsId))
                {
                    throw new Exception("Invalid dsId was provided.");
                }
                client.DsId = dsId;
            }

            if (token != null)
            {
                client.Token = token;
            }

            if (jsonIn["isRequester"] == null && jsonIn["isResponder"] == null)
            {
                throw new Exception("Neither isRequester or isResponder was provided.");
            }

            if (jsonIn["publicKey"] == null || jsonIn["publicKey"].Type != JTokenType.String)
            {
                throw new Exception("publicKey was not provided or it is an invalid type.");
            }
            else
            {
                client.PublicKey = jsonIn["publicKey"].ToObject<string>();
            }

            if (jsonIn["isRequester"] != null && jsonIn["isRequester"].Type == JTokenType.Boolean)
            {
                client.Requester = jsonIn["isRequester"].ToObject<bool>();
            }
            else if (jsonIn["isRequester"] != null)
            {
                throw new Exception("isRequester was provided, but it is an invalid type.");
            }

            if (jsonIn["isResponder"] != null && jsonIn["isResponder"].Type == JTokenType.Boolean)
            {
                client.Responder = jsonIn["isResponder"].ToObject<bool>();
            }
            else if (jsonIn["isResponder"] != null)
            {
                throw new Exception("isResponder was provided, but it is an invalid type.");
            }

            if (jsonIn["version"] == null || jsonIn["version"].Type != JTokenType.String)
            {
                throw new Exception("version was not provided or invalid type.");
            }
            else
            {
                client.ReportingVersion = jsonIn["version"].ToObject<string>();
            }

            if (jsonIn["formats"] != null && jsonIn["formats"].Type != JTokenType.Array)
            {
                throw new Exception("formats was provided, but it is an invalid type.");
            }
            else if (jsonIn["formats"] == null)
            {
                // Server assumes client understands the JSON format if formats isn't provided.
                client.Formats = new List<string>
                {
                    "json"
                };
            }
            else
            {
                var jarray = jsonIn["formats"].ToObject<JArray>();
                var list = new List<string>();
                foreach (JToken jtoken in jarray)
                {
                    if (jtoken.Type != JTokenType.String)
                    {
                        throw new Exception("formats was provided as an array, but contained a value other than a string.");
                    }
                    list.Add(jtoken.ToObject<string>());
                }

                // If the DSLink SDK provides an empty formats list, let's assume they understand JSON.
                // If they don't... I'm not sure what to tell them.
                if (list.Count == 0)
                {
                    client.Formats = new List<string>
                    {
                        "json"
                    };
                }

                client.Formats = list;
            }

            if (jsonIn["enableWebSocketCompression"] != null && jsonIn["enableWebSocketCompression"].Type == JTokenType.Boolean)
            {
                client.WebSocketCompression = jsonIn["enableWebSocketCompression"].ToObject<bool>();
            }
            else if (jsonIn["enableWebSocketCompression"] != null)
            {
                throw new Exception("enableWebSocketCompression was provided, but it is an invalid type.");
            }

            client.Name = _broker.BrokerTree.InitDSLink(client.Name, client.DsId);

            client.HandshakeResponse["dsId"] = _broker.DsId;
            client.HandshakeResponse["publicKey"] = UrlBase64.Encode(_broker.KeyPair.EncodedPublicKey);
            client.HandshakeResponse["wsUri"] = "/ws";
            client.HandshakeResponse["httpUri"] = "/http";
            client.HandshakeResponse["tempKey"] = UrlBase64.Encode(client.TempKey.EncodedPublicKey);
            client.HandshakeResponse["salt"] = Encoding.UTF8.GetString(GenerateSalt(), 0, 32);
            if (client.Responder)
            {
                client.HandshakeResponse["path"] = "/downstream/" + client.Name;
            }
            client.HandshakeResponse["format"] = "json"; // TODO: use actual format

            return client;
        }
        
        /// <summary>
        /// Validates a DSLink identifier.
        /// </summary>
        /// <param name="dsId">DSLink identifier.</param>
        /// <returns>True if the DSID is valid.</returns>
        public static bool ValidateDsId(string dsId)
        {
            // TODO: Check for invalid characters
            bool validLength = 43 <= dsId.Length && dsId.Length <= 128;
            return validLength;
        }

        /// <summary>
        /// Generate a random 32 byte array for salt.
        /// </summary>
        /// <returns>Byte array of length 32.</returns>
        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[32];
            _random.NextBytes(salt);
            return salt;
        }

        /// <summary>
        /// Strips the end of the dsId off to become a more readable name.
        /// </summary>
        /// <param name="dsId">Original dsId.</param>
        /// <returns>Stripped name from long dsId.</returns>
        private static string dsIdToName(string dsId)
        {
            string tmp = dsId.Substring(0, dsId.Length - 43);
            if (tmp.EndsWith("-"))
            {
                tmp = tmp.Substring(0, tmp.Length - 1);
            }
            if (string.IsNullOrEmpty(tmp))
            {
                throw new ArgumentException("Invalid dsId");
            }
            return tmp;
        }
    }
}
