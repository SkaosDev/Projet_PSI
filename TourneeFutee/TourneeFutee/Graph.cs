namespace TourneeFutee
{
    public class Graph
    {
        private bool directed;
        private float noEdgeValue;
        private Matrix adjacencyMatrix;
        private Dictionary<string, int> vertexIndices;
        private Dictionary<string, float> vertexValues;
        
        // --- Construction du graphe ---

        // Contruit un graphe (`directed`=true => orienté)
        // La valeur `noEdgeValue` est le poids modélisant l'absence d'un arc (0 par défaut)
        public Graph(bool isOriented = false, float noEdgeValue = 0, bool directed = false)
        {
            this.directed = isOriented || directed;
            this.noEdgeValue = noEdgeValue;
            this.adjacencyMatrix = new Matrix(0,0);
            this.vertexIndices = new Dictionary<string, int>();
            this.vertexValues = new Dictionary<string, float>();
        }


        // --- Propriétés ---

        // Renvoie le nom du sommet correspondant à l'indice `index` dans la matrice d'adjacence
        public string GetVertexName(int index)
        {
            return vertexIndices.FirstOrDefault(x => x.Value == index).Key;
        }

        // Propriété : ordre du graphe
        // Lecture seule
        public int Order
        {
            get { 
                return vertexIndices.Count; 
            }
        }

        // Propriété : graphe orienté ou non
        // Lecture seule
        public bool Directed
        {
            get {
                return directed;
            }
        }

        public bool IsOriented
        {
            get {
                return directed;
            }
        }

        public int VertexCount
        {
            get {
                return vertexIndices.Count;
            }
        }

        public Matrix AdjacencyMatrix
        {
            get
            {
                return adjacencyMatrix;
            }
        }

        public float NoEdgeValue
        {
            get
            {
                return noEdgeValue;
            }
        }


        // --- Gestion des sommets ---

        public bool ContainsVertex(string name)
        {
            return vertexIndices.ContainsKey(name);
        }

        // Ajoute le sommet de nom `name` et de valeur `value` (0 par défaut) dans le graphe
        // Lève une ArgumentException s'il existe déjà un sommet avec le même nom dans le graphe
        public void AddVertex(string name, float value = 0)
        {
            if (vertexIndices.ContainsKey(name))
            {
                throw new ArgumentException($"Le sommet de nom '{name}' existe déjà dans le graphe.");
            }
            vertexIndices.Add(name, vertexIndices.Count);
            vertexValues.Add(name, value);
            adjacencyMatrix.AddRow(adjacencyMatrix.NbRows);
            adjacencyMatrix.AddColumn(adjacencyMatrix.NbColumns);
        }


        // Supprime le sommet de nom `name` du graphe (et tous les arcs associés)
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public void RemoveVertex(string name)
        {
            if (!vertexIndices.ContainsKey(name)) {
                throw new ArgumentException($"Le sommet de nom '{name}' n'existe pas dans le graphe.");
            }
            int index = vertexIndices[name]; 
            vertexIndices.Remove(name);
            vertexValues.Remove(name);
            adjacencyMatrix.RemoveRow(index);
            adjacencyMatrix.RemoveColumn(index);
            
            // Mise à jour des indices des sommets restants
            foreach (var key in vertexIndices.Keys.ToList())
            { 
                if (vertexIndices[key] > index) {
                    vertexIndices[key]--;
                }
            }
        }

        // Renvoie la valeur du sommet de nom `name`
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public float GetVertexValue(string name)
        {
            if (!vertexValues.ContainsKey(name))
            {
                throw new ArgumentException($"Le sommet de nom '{name}' n'existe pas dans le graphe.");
            }
            return vertexValues[name];
        }

        // Affecte la valeur du sommet de nom `name` à `value`
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public void SetVertexValue(string name, float value)
        {
            if (!vertexValues.ContainsKey(name))
            {
                throw new ArgumentException($"Le sommet de nom '{name}' n'existe pas dans le graphe.");
            }
            vertexValues[name] = value;
        }


        // Renvoie la liste des noms des voisins du sommet de nom `vertexName`
        // (si ce sommet n'a pas de voisins, la liste sera vide)
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public List<string> GetNeighbors(string vertexName)
        {
            List<string> neighborNames = new List<string>();
            
            if (!vertexIndices.ContainsKey(vertexName))
            {
                throw new ArgumentException($"Le sommet de nom '{vertexName}' n'existe pas dans le graphe.");
            }
            
            int vertexIndex = vertexIndices[vertexName];
            for (int j = 0; j < adjacencyMatrix.NbColumns; j++)
            {
                float edgeWeight = adjacencyMatrix.GetValue(vertexIndex, j);
                if (edgeWeight != noEdgeValue)
                {
                    string neighborName = vertexIndices.FirstOrDefault(x => x.Value == j).Key;
                    neighborNames.Add(neighborName);
                }
            }

            return neighborNames;
        }

        // --- Gestion des arcs ---

        /* Ajoute un arc allant du sommet nommé `sourceName` au sommet nommé `destinationName`, avec le poids `weight` (1 par défaut)
         * Si le graphe n'est pas orienté, ajoute aussi l'arc inverse, avec le même poids
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - il existe déjà un arc avec ces extrémités
         */
        public void AddEdge(string sourceName, string destinationName, float weight = 1)
        {
            if (!vertexIndices.ContainsKey(sourceName))
            {
                throw new ArgumentException($"Le sommet source de nom '{sourceName}' n'existe pas dans le graphe.");
            }
            if (!vertexIndices.ContainsKey(destinationName))
            {
                throw new ArgumentException($"Le sommet destination de nom '{destinationName}' n'existe pas dans le graphe.");
            }

            int sourceIndex = vertexIndices[sourceName];
            int destinationIndex = vertexIndices[destinationName];

            if (adjacencyMatrix.GetValue(sourceIndex, destinationIndex) != noEdgeValue)
            {
                throw new ArgumentException($"Un arc allant de '{sourceName}' à '{destinationName}' existe déjà dans le graphe.");
            }

            adjacencyMatrix.SetValue(sourceIndex, destinationIndex, weight);

            if (!directed)
            {
                adjacencyMatrix.SetValue(destinationIndex, sourceIndex, weight);
            }
        }

        /* Supprime l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName` du graphe
         * Si le graphe n'est pas orienté, supprime aussi l'arc inverse
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - l'arc n'existe pas
         */
        public void RemoveEdge(string sourceName, string destinationName)
        {
            if (!vertexIndices.ContainsKey(sourceName))
            {
                throw new ArgumentException($"Le sommet source de nom '{sourceName}' n'existe pas dans le graphe.");
            }
            if (!vertexIndices.ContainsKey(destinationName))
            {
                throw new ArgumentException($"Le sommet destination de nom '{destinationName}' n'existe pas dans le graphe.");
            }

            int sourceIndex = vertexIndices[sourceName];
            int destinationIndex = vertexIndices[destinationName];

            if (adjacencyMatrix.GetValue(sourceIndex, destinationIndex) == noEdgeValue)
            {
                throw new ArgumentException($"L'arc allant de '{sourceName}' à '{destinationName}' n'existe pas dans le graphe.");
            }

            adjacencyMatrix.SetValue(sourceIndex, destinationIndex, noEdgeValue);

            if (!directed)
            {
                adjacencyMatrix.SetValue(destinationIndex, sourceIndex, noEdgeValue);
            }
        }

        /* Renvoie le poids de l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName`
         * Si le graphe n'est pas orienté, GetEdgeWeight(A, B) = GetEdgeWeight(B, A) 
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - l'arc n'existe pas
         */
        public float GetEdgeWeight(string sourceName, string destinationName)
        {
            if(!vertexIndices.ContainsKey(sourceName))
            {
                throw new ArgumentException($"Le sommet source de nom '{sourceName}' n'existe pas dans le graphe.");
            }
            if(!vertexIndices.ContainsKey(destinationName))
            {
                throw new ArgumentException($"Le sommet destination de nom '{destinationName}' n'existe pas dans le graphe.");
            }
            
            int sourceIndex = vertexIndices[sourceName];
            int destinationIndex = vertexIndices[destinationName];
            
            if(adjacencyMatrix.GetValue(sourceIndex, destinationIndex) == noEdgeValue)
            {
                throw new ArgumentException($"L'arc allant de '{sourceName}' à '{destinationName}' n'existe pas dans le graphe.");
            }
            
            return adjacencyMatrix.GetValue(sourceIndex, destinationIndex);
        }

        /* Affecte le poids l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName` à `weight` 
         * Si le graphe n'est pas orienté, affecte le même poids à l'arc inverse
         * Lève une ArgumentException si un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         */
        public void SetEdgeWeight(string sourceName, string destinationName, float weight)
        {
            if(!vertexIndices.ContainsKey(sourceName))
            {
                throw new ArgumentException($"Le sommet source de nom '{sourceName}' n'existe pas dans le graphe.");
            }
            if(!vertexIndices.ContainsKey(destinationName))
            {
                throw new ArgumentException($"Le sommet destination de nom '{destinationName}' n'existe pas dans le graphe.");
            }
            
            int sourceIndex = vertexIndices[sourceName];
            int destinationIndex = vertexIndices[destinationName];
            
            if (adjacencyMatrix.GetValue(sourceIndex, destinationIndex) == noEdgeValue)
            {
                throw new ArgumentException($"L'arc allant de '{sourceName}' à '{destinationName}' n'existe pas dans le graphe.");
            }
            
            adjacencyMatrix.SetValue(sourceIndex, destinationIndex, weight);
            
            if(!directed)
            {
                adjacencyMatrix.SetValue(destinationIndex, sourceIndex, weight);
            }
        }
    }


}
