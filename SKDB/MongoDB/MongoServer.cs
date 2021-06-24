using System;
using System.Security;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using SKDB.Boundary;

namespace SKDB.MongoDB
{
    /// <summary>
    /// Represents a MongoDB server based on MongoDB 4.2+.
    /// <br/><br/>
    /// By default it de-/serializes classes using Bson attributes, e.g. BsonIgnore etc.
    /// <br/><br/>
    /// You can pass class mappings and serializer registrations via the public constructor callback function
    /// For example:
    /// <br/><br/>
    /// <code>BsonClassMap.RegisterClassMap&lt;Session&gt;(cm =><br/>
    /// {<br/>
    ///     cm.AutoMap();<br/>
    ///     cm.SetIsRootClass(true);<br/>
    /// });<br/>
    /// BsonClassMap.RegisterClassMap&lt;VideoSession&gt;();<br/>
    /// BsonClassMap.RegisterClassMap&lt;PhotoSession&gt;();<br/>
    /// BsonClassMap.RegisterClassMap&lt;MedicalReportSession&gt;();
    /// </code>
    /// </summary>
    internal class MongoServer : IDatabaseServer
    {
        #region api_internals

        private readonly MongoClientSettings mClientSettings;

        #region server_hooks

        private void CmdStartHandler(CommandStartedEvent cmd)
        {
            MongoServer.WriteToConsole(cmd.Command, "request", cmd.CommandName);
        }

        private void CmdSuccessHandler(CommandSucceededEvent cmd)
        {
            MongoServer.WriteToConsole(cmd.Reply, "response", cmd.CommandName);
        }

        private void CmdFailedHandler(CommandFailedEvent cmd)
        {
            MongoServer.WriteToConsole(cmd.ToBsonDocument(), "error", cmd.Failure.Message);
        }

        private void CmdConnectionLostHandler(ConnectionClosedEvent cmd)
        {
            MongoServer.WriteToConsole(cmd.ToBsonDocument(), "connection id", cmd.ConnectionId.ToString());
        }

        private void CmdConnectionFailHandler(ConnectionFailedEvent cmd)
        {
            MongoServer.WriteToConsole(cmd.ToBsonDocument(), "connection failed", cmd.Exception.Message);
        }

        private static void WriteToConsole(BsonDocument data, string type, string commandName)
        {
            Console.WriteLine($"************** {commandName}: {type} **************");
            Console.WriteLine(data.ToJson(new JsonWriterSettings {Indent = true}));
        }

        #endregion

        private static void RegisterConventions(string applicationName)
        {
            var conventions = new ConventionPack
            {
                // All enums shall be serialized as strings
                new EnumRepresentationConvention(BsonType.String),
                // All element names shall be lower case
                new MongoLowerCaseConvention()
            };

            ConventionRegistry.Register($"{applicationName}Conventions", conventions, t => true);
        }

        #endregion

        internal MongoServer(string applicationName, Action mappings = null)
            : this(applicationName, "localhost", 27017, mappings) { }

        internal MongoServer(string applicationName,
                                 string host,
                                 int port,
                                 Action mappings = null)
        {
            mClientSettings = new MongoClientSettings
            {
                ApplicationName = applicationName,
                ConnectionMode  = ConnectionMode.Direct,
                ConnectTimeout  = TimeSpan.FromSeconds(5),
                Server          = new MongoServerAddress(host, port),
                ClusterConfigurator = builder =>
                {
                    //builder.Subscribe(new SingleEventSubscriber<CommandStartedEvent>(CmdStartHandler));
                    //builder.Subscribe(new SingleEventSubscriber<CommandSucceededEvent>(CmdSuccessHandler));
                    //builder.Subscribe(new SingleEventSubscriber<CommandFailedEvent>(CmdFailedHandler));
                    //builder.Subscribe(new SingleEventSubscriber<ConnectionClosedEvent>(CmdConnectionLostHandler));
                    //builder.Subscribe(new SingleEventSubscriber<ConnectionFailedEvent>(CmdConnectionFailHandler));
                }
            };

            MongoServer.RegisterConventions(applicationName);
            mappings?.Invoke();

            if (BsonSerializer.LookupSerializer<MongoDatabaseIdSerializer>() is null)
            {
                BsonSerializer.RegisterSerializer(typeof(IDatabaseId), new MongoDatabaseIdSerializer());
            }
        }

        private static MongoClient CreateClient(MongoClientSettings settings, string dbName)
        {
            var client = new global::MongoDB.Driver.MongoClient(settings);
            if (client is null)
            {
                throw new NullReferenceException("Could not create a mongo client with the given settings");
            }
            return new MongoClient(client, dbName);
        }

        public IDatabaseClient CreateClient(string dbName, string userName, SecureString password)
        {
            var settings = mClientSettings.Clone();
            settings.Credential = MongoCredential.CreateCredential(dbName, userName, password);

            return MongoServer.CreateClient(settings, dbName);
        }

        public IDatabaseClient CreateClient(string dbName)
        {
            return MongoServer.CreateClient(mClientSettings.Clone(), dbName);
        }
    }
}