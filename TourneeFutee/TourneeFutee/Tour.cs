namespace TourneeFutee
{
    // Modélise une tournée dans le cadre du problème du voyageur de commerce
    public class Tour
    {
        private List<(string source, string destination)> segments;
        private float cost;
        
        public Tour(List<(string source, string destination)> segments, float cost)
        {
            this.segments = segments;
            this.cost = cost;
        }
        
        public Tour()
        {
            this.segments = new List<(string source, string destination)>();
            this.cost = 0.0f;
        }

        // propriétés

        // Coût total de la tournée
        public float Cost
        {
            get {
                return this.cost;
            }
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

        public void AddSegment((string source, string destination) segment, float segmentCost)
        {
            this.segments.Add(segment);
            this.cost += segmentCost;
        }
        
        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }
}
