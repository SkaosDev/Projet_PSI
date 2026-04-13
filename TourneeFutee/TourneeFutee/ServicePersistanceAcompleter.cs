using System;
using MySql.Data.MySqlClient;

namespace TourneeFutee
{
    /// <summary>
    /// Service de persistance permettant de sauvegarder et charger
    /// des graphes et des tournées dans une base de données MySQL.
    /// </summary>
    public class ServicePersistance
    {
        // ─────────────────────────────────────────────────────────────────────
        // Attributs privés
        // ─────────────────────────────────────────────────────────────────────

        private readonly string _connectionString;
        private MySqlConnection conn;

        // TODO : si vous avez besoin de maintenir une connexion ouverte,
        //        ajoutez un attribut MySqlConnection ici.

        // ─────────────────────────────────────────────────────────────────────
        // Constructeur
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Instancie un service de persistance et se connecte automatiquement
        /// à la base de données <paramref name="dbname"/> sur le serveur
        /// à l'adresse IP <paramref name="serverIp"/>.
        /// Les identifiants sont définis par <paramref name="user"/> (utilisateur)
        /// et <paramref name="pwd"/> (mot de passe).
        /// </summary>
        /// <param name="serverIp">Adresse IP du serveur MySQL.</param>
        /// <param name="dbname">Nom de la base de données.</param>
        /// <param name="user">Nom d'utilisateur.</param>
        /// <param name="pwd">Mot de passe.</param>
        /// <exception cref="Exception">Levée si la connexion échoue.</exception>
        public ServicePersistance(string serverIp, string dbname, string user, string pwd)
        {
            string certPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ca.pem");

            string host = serverIp;
            string port = "3306";
            if (serverIp.Contains(":"))
            {
                var parts = serverIp.Split(':');
                host = parts[0];
                port = parts[1];
            }

            _connectionString = $"server={host};port={port};database={dbname};uid={user};pwd={pwd};SslMode=Required;SslCa={certPath};";

            conn = OpenConnection();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Méthodes publiques
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sauvegarde le graphe <paramref name="g"/> en base de données
        /// (sommets et arcs inclus) et renvoie son identifiant.
        /// </summary>
        /// <param name="g">Le graphe à sauvegarder.</param>
        /// <returns>Identifiant du graphe en base de données (AUTO_INCREMENT).</returns>
        
        public uint SaveGraph(Graph g)
        {
            using (var conn = OpenConnection())
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    // 1. Insertion du Graphe
                    var cmdGraphe = new MySqlCommand("INSERT INTO Graphe (est_oriente) VALUES (@dir); SELECT LAST_INSERT_ID();", conn, transaction);
                    cmdGraphe.Parameters.AddWithValue("@dir", g.Directed);
                    uint gid = Convert.ToUInt32(cmdGraphe.ExecuteScalar());

                    // Insertion des Sommets
                    // On stocke la correspondance : Nom du sommet (C#) -> ID auto-incrémenté (SQL)
                    var sommetNameToId = new Dictionary<string, uint>();

                    var cmdSommet = new MySqlCommand("INSERT INTO Sommet(nom, valeur, graphe_id) VALUES (@nom, @val, @gid); SELECT LAST_INSERT_ID();", conn, transaction);
                    cmdSommet.Parameters.Add("@nom", MySqlDbType.VarChar);
                    cmdSommet.Parameters.Add("@val", MySqlDbType.Float);
                    cmdSommet.Parameters.AddWithValue("@gid", gid);

                    // On itère sur les noms 
                    for (int i = 0; i < g.Order; i++)
                    {
                        string sName = g.GetVertexName(i);
                        float sVal = g.GetVertexValue(sName);

                        cmdSommet.Parameters["@nom"].Value = sName;
                        cmdSommet.Parameters["@val"].Value = sVal;
                        
                        uint dbId = Convert.ToUInt32(cmdSommet.ExecuteScalar());
                        sommetNameToId.Add(sName, dbId);
                    }

                    // Insertion des Arcs
                    var cmdArc = new MySqlCommand(
                        "INSERT INTO Arc(sommet_source, sommet_dest, poids, graphe_id) VALUES (@src, @dest, @p, @gid)", 
                        conn, transaction);
                    cmdArc.Parameters.Add("@src", MySqlDbType.UInt32);
                    cmdArc.Parameters.Add("@dest", MySqlDbType.UInt32);
                    cmdArc.Parameters.Add("@p", MySqlDbType.Float);
                    cmdArc.Parameters.AddWithValue("@gid", gid);

                    for (int i = 0; i < g.Order; i++)
                    {
                        string sourceName = g.GetVertexName(i);
                        for (int j = 0; j < g.Order; j++)
                        {
                            string destName = g.GetVertexName(j);
                            
                            // On récupère le poids via la matrice 
                            float weight = g.AdjacencyMatrix.GetValue(i, j);

                            // On n'insère que si l'arc existe 
                            if (weight != g.NoEdgeValue) 
                            {
                                cmdArc.Parameters["@src"].Value = sommetNameToId[sourceName];
                                cmdArc.Parameters["@dest"].Value = sommetNameToId[destName];
                                cmdArc.Parameters["@p"].Value = weight;
                                cmdArc.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();
                    return gid;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Charge depuis la base de données le graphe identifié par <paramref name="id"/>
        /// et renvoie une instance de la classe <see cref="Graph"/>.
        /// </summary>
        /// <param name="id">Identifiant du graphe à charger.</param>
        /// <returns>Instance de <see cref="Graph"/> reconstituée.</returns>
        public Graph LoadGraph(uint id)
        {
            // TODO : implémenter le chargement du graphe
            //
            // Ordre recommandé :
            //   1. SELECT dans Graphe WHERE id = @id -> récupérer IsOriented, etc.
            //   2. SELECT dans Sommet WHERE graphe_id = @id -> reconstruire les sommets
            //      (respecter l'ordre d'insertion pour que les indices de la matrice
            //       correspondent à ceux sauvegardés)
            //   3. SELECT dans Arc WHERE graphe_id = @id -> reconstruire la matrice
            //      d'adjacence en utilisant les correspondances sommet_id <-> indice
            
            var cmdGraphe = new MySqlCommand("SELECT * FROM Graphe WHERE id = @id", conn);
            cmdGraphe.Parameters.AddWithValue("@id", id);
            var readerGraphe = cmdGraphe.ExecuteReader();
            readerGraphe.Read();
            bool isOriented = readerGraphe.GetBoolean("est_oriente");
            readerGraphe.Close();

            Graph graph = new Graph(isOriented);
            
            var idToName = new Dictionary<uint, string>();
            var cmdSommets = new MySqlCommand("SELECT * FROM Sommet WHERE graphe_id = @id ORDER BY id", conn);
            cmdSommets.Parameters.AddWithValue("@id", id);
            var readerSommets = cmdSommets.ExecuteReader();
            while (readerSommets.Read())
            {
                uint sommetId = readerSommets.GetUInt32("id");
                string nom = readerSommets.GetString("nom");
                string valeur = readerSommets.GetString("valeur");
                graph.AddVertex(nom, float.Parse(valeur));
                idToName[sommetId] = nom;
            }
            readerSommets.Close();
            
            var cmdArcs = new MySqlCommand("SELECT * FROM Arc WHERE graphe_id = @id", conn);
            cmdArcs.Parameters.AddWithValue("@id", id);
            var readerArcs = cmdArcs.ExecuteReader();
            while (readerArcs.Read())
            {
                uint sourceId = readerArcs.GetUInt32("sommet_source");
                uint destId = readerArcs.GetUInt32("sommet_dest");
                float poids = readerArcs.GetFloat("poids");
                string nomSource = idToName[sourceId];
                string nomDest = idToName[destId];
                graph.AddEdge(nomSource, nomDest, poids);
            }
            readerArcs.Close();

            return graph;
        }

        /// <summary>
        /// Sauvegarde la tournée <paramref name="t"/> (effectuée dans le graphe
        /// identifié par <paramref name="graphId"/>) en base de données
        /// et renvoie son identifiant.
        /// </summary>
        /// <param name="graphId">Identifiant BdD du graphe dans lequel la tournée a été calculée.</param>
        /// <param name="t">La tournée à sauvegarder.</param>
        /// <returns>Identifiant de la tournée en base de données (AUTO_INCREMENT).</returns>
        public uint SaveTour(uint graphId, Tour t)
        {
            // TODO : implémenter la sauvegarde de la tournée
            //
            // Ordre recommandé :
            //   1. INSERT dans Tournee (cout_total, graphe_id) -> récupérer l'id
            //   2. Pour chaque sommet de la séquence (avec son numéro d'ordre) :
            //      INSERT dans EtapeTournee (tournee_id, numero_ordre, sommet_id)
            //
            // Attention : conserver l'ordre des étapes est essentiel pour
            //             pouvoir reconstruire la tournée fidèlement au chargement.

            throw new NotImplementedException("SaveTour non implémenté.");
        }

        /// <summary>
        /// Charge depuis la base de données la tournée identifiée par <paramref name="id"/>
        /// et renvoie une instance de la classe <see cref="Tour"/>.
        /// </summary>
        /// <param name="id">Identifiant de la tournée à charger.</param>
        /// <returns>Instance de <see cref="Tour"/> reconstituée.</returns>
        public Tour LoadTour(uint id)
        {
            // TODO : implémenter le chargement de la tournée
            //
            // Ordre recommandé :
            //   1. SELECT dans Tournee WHERE id = @id -> récupérer cout_total et graphe_id
            //   2. SELECT dans EtapeTournee JOIN Sommet WHERE tournee_id = @id
            //      ORDER BY numero_ordre -> reconstruire la séquence ordonnée de sommets
            //   3. Construire et retourner l'instance Tour

            throw new NotImplementedException("LoadTour non implémenté.");
        }

        // ─────────────────────────────────────────────────────────────────────
        // Méthodes utilitaires privées (à compléter selon vos besoins)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Crée et retourne une nouvelle connexion MySQL ouverte.
        /// Encadrez toujours l'appel dans un bloc using pour garantir la fermeture.
        /// </summary>
        private MySqlConnection OpenConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
