namespace WPFTextConverter
{
    public struct Detail
    {
        public double Width;
        public double Height;
        public int Quantity;
        public string Material;
        public string Hint;
        public bool IsGrainDirectionReversed;
        public bool HasLeftEdge;
        public bool HasTopEdge;
        public bool HasRightEdge;
        public bool HasBottomEdge;
        public string Description;
        public string LoniraEgdes
        {
            get
            {
                return this.CreateLoniraEdges();
            }
        }

        public Detail(double height, double width, int quantity, string material, bool isGrainDirectionReversed, string hint, bool hasTopEdge, bool hasBottomEdge, bool hasRightEdge, bool hasLeftEdge, string description)
        {
            this.Height = height;
            this.Width = width;
            this.Quantity = quantity;
            this.Material = material;
            this.IsGrainDirectionReversed = isGrainDirectionReversed;
            this.Hint = hint;
            this.HasLeftEdge = hasLeftEdge;
            this.HasTopEdge = hasTopEdge;
            this.HasRightEdge = hasRightEdge;
            this.HasBottomEdge = hasBottomEdge;
            this.Description = description;

            if (this.IsGrainDirectionReversed)
            {
                var temp = this.Width;
                this.Width = this.Height;
                this.Height = temp;
            }
        }

        private string CreateLoniraEdges()
        {
            int longEdgeCount = 0;
            int shortEdgeCount = 0;

            if (this.Width >= this.Height)
            {
                if (this.HasLeftEdge)
                {
                    shortEdgeCount++;
                }
                if (this.HasTopEdge)
                {
                    longEdgeCount++;
                }
                if (this.HasRightEdge)
                {
                    shortEdgeCount++;
                }
                if (this.HasBottomEdge)
                {
                    longEdgeCount++;
                }
            }
            else
            {
                if (this.HasLeftEdge)
                {
                    longEdgeCount++;
                }
                if (this.HasTopEdge)
                {
                    shortEdgeCount++;
                }
                if (this.HasRightEdge)
                {
                    longEdgeCount++;
                }
                if (this.HasBottomEdge)
                {
                    shortEdgeCount++;
                }
            }

            if (shortEdgeCount == 0 && longEdgeCount == 0)
            {
                return string.Empty;
            }
            if (shortEdgeCount == 0 && longEdgeCount != 0)
            {
                return $"{longEdgeCount} d";
            }
            if (longEdgeCount == 0 && shortEdgeCount != 0)
            {
                return $"{shortEdgeCount} k";
            }
            return $"{shortEdgeCount} k {longEdgeCount} d";
        }
    }
}