namespace ConsoleApp1
{
    public interface IRoute
    {
        double Cost();
        string Descrb();
    }
    public class SingleRoute : IRoute
    {
        public List<string> Points { get; } = new List<string>();
        public string TransportType { get; set; }
        public double BaseCost { get; set; } = 100.0;

        public double Cost() => BaseCost;

        public string Descrb() =>
            $"Маршрут: {string.Join(" → ", Points)} " +
            $"[Транспорт: {TransportType}, Стоимость: {BaseCost}]";
    }
    public class CompositeRoute : IRoute
    {
        private readonly List<IRoute> _routes = new List<IRoute>();

        public void AddR(IRoute route) => _routes.Add(route);
        public void RemoveR(IRoute route) => _routes.Remove(route);
        public double Cost()
        {
            double total = 0;
            foreach (var route in _routes)
                total += route.Cost();
            return total;
        }
        public string Descrb()
        {
            var descript = "Составной маршрут:\n";
            foreach (var route in _routes)
                descript += $" - {route.Descrb()}\n";
            return descript;
        }
    } 
    public abstract class RDecort : IRoute
    {
        protected IRoute _route;

        protected RDecort(IRoute route) => _route = route;
        public virtual double Cost() => _route.Cost();
        public virtual string Descrb() => _route.Descrb();
    }
    public class InsRDeroct : RDecort
    {
        private const double InsuranceCost = 50.0;

        public InsRDeroct(IRoute route) : base(route) { }

        public override double Cost() =>
            _route.Cost() + InsuranceCost;

        public override string Descrb() =>
            $"{_route.Descrb()} [Страховка: +{InsuranceCost}]";
    }
    public class RefDeroct : RDecort
    {
        private const double RDeroctCost = 80.0;

        public RefDeroct(IRoute route) : base(route) { }

        public override double Cost() =>
            _route.Cost() + RDeroctCost;

        public override string Descrb() =>
            $"{_route.Descrb()} [Холодовая цепь: +{RDeroctCost}]";
    }
    public class ExpressesRoute : RDecort
    {
        private const double ExpresMult = 1.5;

        public ExpressesRoute(IRoute route) : base(route) { }

        public override double Cost() =>
            _route.Cost() * ExpresMult;

        public override string Descrb() =>
            $"{_route.Descrb()} [Срочная доставка: x{ExpresMult}]";
    }  
    public class RBuilder
    {
        private SingleRoute _route = new SingleRoute();
        public RBuilder AddPoint(string point)
        {
            _route.Points.Add(point);
            return this;
        }
        public RBuilder SetTransport(string transport)
        {
            _route.TransportType = transport;
            return this;
        }
        public RBuilder SetBaseCost(double cost)
        {
            _route.BaseCost = cost;
            return this;
        }
        public IRoute Build()
        {
            if (_route.Points.Count < 2)
                throw new InvalidOperationException("Маршрут должен содержать минимум 2 точки");

            if (string.IsNullOrEmpty(_route.TransportType))
                throw new InvalidOperationException("Транспорт не указан");

            var result = _route;
            _route = new SingleRoute();
            return result;
        }
    }
    class Program
    {
        static void Main()
        {
            var builder = new RBuilder();
            var route1 = builder
                .AddPoint("Киев")
                .AddPoint("Одесса")
                .SetTransport("Грузовик")
                .SetBaseCost(150.0)
                .Build();
  
            var insuredRoute = new InsRDeroct(route1);
            var expressRoute = new ExpressesRoute(insuredRoute);
     
            var route2 = new RBuilder()
                .AddPoint("Одесса")
                .AddPoint("Стамбул")
                .SetTransport("Корабль")
                .SetBaseCost(300.0)
                .Build();

            var refrigeratedRoute = new RefDeroct(route2);
       
            var composite = new CompositeRoute();
            composite.AddR(expressRoute);
            composite.AddR(refrigeratedRoute);

            Console.WriteLine("Общая стоимость: " + composite.Cost());
            Console.WriteLine(composite.Descrb());
        }
    }
}

