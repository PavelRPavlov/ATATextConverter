using System.Collections.Generic;

namespace ATAFurniture.Server.Models;
public class LoniraDetail : IKroikoDetail
{
    public double Width { get; set; }
    public double Height { get; set; }
    public int Quantity { get; set; }
    public string LoniraEdges { get; set; }
    public string Note { get; set; }
}

public interface IKroikoDetail
{
    public double Width { get; set; }
    public double Height { get; set; }
    public int Quantity { get; set; }
}

public class KroikoFile
{
    public string FileName { get; set; }
    public List<IKroikoDetail> Details { get; set; }
}