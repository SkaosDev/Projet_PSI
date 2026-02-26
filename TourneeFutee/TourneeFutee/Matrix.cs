namespace TourneeFutee
{
    public class Matrix
    {
        private int nbRows;
        private int nbColumns;
        private float defaultValue;
        private List<List<float>> matrice;
        
        

        #region MatrixConstructeurs
        
        /* Crée une matrice de dimensions `nbRows` x `nbColumns`.
         * Toutes les cases de cette matrice sont remplies avec `defaultValue`.
         * Lève une ArgumentOutOfRangeException si une des dimensions est négative
         */

        public Matrix(int nbRows, int nbColumns, float defaultValue)
        {
            if (nbRows < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nbRows), "Le nombre de lignes ne peut pas être négatif.");
            }
            if (nbColumns < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nbColumns), "Le nombre de colonnes ne peut pas être négatif.");
            }
            
            this.nbRows = nbRows;
            this.nbColumns = nbColumns;
            this.defaultValue = defaultValue;

            this.matrice = new List<List<float>>(this.nbRows);
            
            for (int i = 0; i < this.nbRows; i++)
            {
                List<float> ligne = new List<float>(this.nbColumns);
                
                for (int j = 0; j < this.nbColumns; j++)
                {
                    ligne.Add(this.defaultValue);
                }

                matrice.Add(ligne);

            }
            
        } // constructeur exigeant
        
        public Matrix(int nbRows, int nbColumns)
        {
            if (nbRows < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nbRows), "Le nombre de lignes ne peut pas être négatif.");
            }
            if (nbColumns < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nbColumns), "Le nombre de colonnes ne peut pas être négatif.");
            }
            
            this.nbRows = nbRows;
            this.nbColumns = nbColumns;
            this.defaultValue = 0;

            List<List<float>> matrice = new List<List<float>>(this.nbRows);
            
            for (int i = 0; i < this.nbRows; i++)
            {
                List<float> ligne = new List<float>(this.nbColumns);
                
                for (int j = 0; j < this.nbColumns; j++)
                {
                    ligne.Add(this.defaultValue);
                }

                matrice.Add(ligne);

            }
            
        } // constructeur avec dv = 0

        #endregion
        


        #region MatrixPropriétés

        // Propriété : valeur par défaut utilisée pour remplir les nouvelles cases
        // Lecture seule
        public float DefaultValue
        {
            get { return defaultValue; }
            // pas de set
        }

        // Propriété : nombre de lignes
        // Lecture seule
        public int NbRows
        {
            get { return nbRows; }
            // pas de set
        }

        // Propriété : nombre de colonnes
        // Lecture seule
        public int NbColumns
        {
            get { return nbColumns; }
            // pas de set
        }

        #endregion



        #region MatrixMethodes

        /* Insère une ligne à l'indice `i`. Décale les lignes suivantes vers le bas.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `i` = NbRows, insère une ligne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
         */
        public void AddRow(int i)
        {

            if (i < 0 || i > this.nbRows) //vérifier si i positif et <= nbLignes de la matrice
            {
                throw new ArgumentOutOfRangeException("L'index est invalide");
            }
            
            List<float> nvlLigne = new List<float>(this.nbColumns); //créer la row
            
            for (int j = 0; j < this.nbColumns; j++)  //initialiser ts les éléms à defaultValue
            {
                nvlLigne.Add(this.defaultValue);
            }

            this.matrice.Insert(i, nvlLigne); // insérer la ligne au bon indice.
            
        }

        /* Insère une colonne à l'indice `j`. Décale les colonnes suivantes vers la droite.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `j` = NbColums, insère une colonne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
         */
        public void AddColumn(int j)
        {
            if (this.matrice.Count <= 0) // vérifier non vide
            {
                throw new InvalidOperationException("La matrice est vide");
            }

            if (j < 0 || j > this.nbColumns)
            {
                throw new ArgumentOutOfRangeException("L'index est invalide"); // vérifier index dans le range
            }

            foreach (List<float> ligne in this.matrice) // itérer sur chaque ligne et insérer en j-eme place
            {
                ligne.Insert(j, defaultValue);
            }
            
        }

        // Supprime la ligne à l'indice `i`. Décale les lignes suivantes vers le haut.
        // Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
        public void RemoveRow(int i)
        {
            if (i < 0 || i > this.nbRows) //vérifier si i positif et <= nbLignes de la matrice
            {
                throw new ArgumentOutOfRangeException("L'index est invalide");
            }
            
            this.matrice.RemoveAt(i);
        }

        // Supprime la colonne à l'indice `j`. Décale les colonnes suivantes vers la gauche.
        // Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
        public void RemoveColumn(int j)
        {
            if (this.matrice.Count <= 0) // vérifier non vide
            {
                throw new InvalidOperationException("La matrice est vide");
            }

            if (j < 0 || j > this.nbColumns)
            {
                throw new ArgumentOutOfRangeException("L'index est invalide"); // vérifier index dans le range
            }

            foreach (List<float> ligne in this.matrice)
            {
                ligne.RemoveAt(j);
            }
        }

        // Renvoie la valeur à la ligne `i` et colonne `j`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public float GetValue(int i, int j)
        {
            if (i < 0 || i >= this.nbRows || j < 0 || j >= this.nbColumns)
            {
                throw new ArgumentOutOfRangeException("Indice(s) invalides");
            }

            return this.matrice[i][j];
        }

        // Affecte la valeur à la ligne `i` et colonne `j` à `v`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public void SetValue(int i, int j, float v)
        {
            if (i < 0 || i >= this.nbRows || j < 0 || j >= this.nbColumns)
            {
                throw new ArgumentOutOfRangeException("Indice(s) invalides");
            }
            
            this.matrice[i][j] = v;
        }

        // Affiche la matrice
        public void Print()
        {
            // TODO : implémenter
            foreach (List<float> ligne in this.matrice)
            {
                foreach (float e in ligne)
                {
                    Console.Write(e + " ");
                }
                Console.WriteLine();
            }
            
        }

        #endregion

        

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }


}
