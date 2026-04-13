namespace TourneeFutee
{
    // Modélise une tournée dans le cadre du problème du voyageur de commerce
    public class Tour
    {
        private List<(string source, string destination)> segments;
        private List<string> vertices;
        private float cost;
        
        public Tour(List<(string source, string destination)> segments, float cost)
        {
            this.segments = segments;
            this.vertices = new List<string>();
            this.cost = cost;
        }
        
        public Tour()
        {
            this.segments = new List<(string source, string destination)>();
            this.vertices = new List<string>();
            this.cost = 0.0f;
        }

        public Tour(List<string> vertices, float cost)
        {
            this.segments = new List<(string source, string destination)>();
            this.vertices = new List<string>(vertices);
            this.cost = cost;
            for (int i = 0; i < vertices.Count - 1; i++)
            {
                this.segments.Add((vertices[i], vertices[i + 1]));
            }
        }

        // propriétés

        // Coût total de la tournée
        public float Cost
        {
            get {
                return this.cost;
            }
        }

        public IList<string> Vertices
        {
            get { return this.vertices; }
        }

        // Nombre de trajets dans la tournée
        public int NbSegments
        {
            get {
                return this.segments.Count;
            }
        }

        // Renvoie vrai si la tournée contient le trajet `source`->`destination`
        public bool ContainsSegment((string source, string destination) segment)
        {
            return this.segments.Contains(segment);
        }


        // Affiche les informations sur la tournée : coût total et trajets
        public void Print()
        {
            Console.WriteLine($"Coût total de la tournée : {this.cost}");
            Console.WriteLine("Trajets :");
            foreach ((string source, string destination) segment in this.segments)
            {
                Console.WriteLine($"{segment.source} -> {segment.destination}");
            }
        }

        public string GetVertices(int index)
        {
            if (index >= 0 && index < this.vertices.Count - 1)
            {
                return this.segments[index].source;
            }else if(index == this.vertices.Count - 1)
            {
                return this.segments[index - 1].destination;
            }
            return "";
        }
        
        public void AddSegment((string source, string destination) segment, float segmentCost)
        {
            this.segments.Add(segment);
            this.cost += segmentCost;
        }
        
        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }
}
