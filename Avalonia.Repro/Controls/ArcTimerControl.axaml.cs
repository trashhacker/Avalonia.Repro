using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Threading;

namespace Avalonia.Repro.Controls;

public partial class ArcTimerControl : UserControl
{
    class IndicatorHelper
    {
        public PathFigure Figure { get; }
        public ArcSegment Segment { get; }

        public IndicatorHelper(PathFigure figure, ArcSegment segment)
        {
            Segment = segment ?? throw new ArgumentNullException(nameof(segment));
            Figure = figure ?? throw new ArgumentNullException(nameof(figure));
        }
        public void SetPosition(Point startpoint, Point endpoint, double radius, double arcangle)
        {
            Figure.StartPoint = startpoint;
            Segment.Size = new Size(radius, radius);
            Segment.RotationAngle = arcangle;
            Segment.Point = endpoint;
        }
    }

    public static readonly StyledProperty<IImmutableBrush> SecondsStrokeProperty = AvaloniaProperty.Register<ArcTimerControl, IImmutableBrush>(nameof(SecondsStroke), Brushes.LightGray);
    public static readonly StyledProperty<IImmutableBrush> MinutesStrokeProperty = AvaloniaProperty.Register<ArcTimerControl, IImmutableBrush>(nameof(MinutesStroke), Brushes.Silver);
    public static readonly StyledProperty<IImmutableBrush> HoursStrokeProperty = AvaloniaProperty.Register<ArcTimerControl, IImmutableBrush>(nameof(HoursStroke), Brushes.DarkGray);
    public static readonly StyledProperty<IImmutableBrush> SecondsHandProperty = AvaloniaProperty.Register<ArcTimerControl, IImmutableBrush>(nameof(SecondsHand), Brushes.LightGray);
    public static readonly StyledProperty<IImmutableBrush> MinutesHandProperty = AvaloniaProperty.Register<ArcTimerControl, IImmutableBrush>(nameof(MinutesHand), Brushes.Silver);
    public static readonly StyledProperty<IImmutableBrush> HoursHandProperty = AvaloniaProperty.Register<ArcTimerControl, IImmutableBrush>(nameof(HoursHand), Brushes.DarkGray);
    public static readonly StyledProperty<IImmutableBrush> HoverProperty = AvaloniaProperty.Register<ArcTimerControl, IImmutableBrush>(nameof(Hover), Brushes.Gray);

    public static readonly StyledProperty<bool> SecondsVisibilityProperty = AvaloniaProperty.Register<ArcTimerControl, bool>(nameof(SecondsVisibility), true);
    public static readonly StyledProperty<bool> MinutesVisibilityProperty = AvaloniaProperty.Register<ArcTimerControl, bool>(nameof(MinutesVisibility), true);
    public static readonly StyledProperty<bool> HoursVisibilityProperty = AvaloniaProperty.Register<ArcTimerControl, bool>(nameof(HoursVisibility), true);
    public static readonly StyledProperty<bool> WatchModeProperty = AvaloniaProperty.Register<ArcTimerControl, bool>(nameof(WatchMode), false);

    public static readonly StyledProperty<double> SecondsProperty = AvaloniaProperty.Register<ArcTimerControl, double>(nameof(Seconds));
    public static readonly StyledProperty<double> MinutesProperty = AvaloniaProperty.Register<ArcTimerControl, double>(nameof(Minutes));
    public static readonly StyledProperty<double> HoursProperty = AvaloniaProperty.Register<ArcTimerControl, double>(nameof(Hours));

    public IImmutableBrush SecondsStroke
    {
        get => GetValue(SecondsStrokeProperty);
        set => SetValue(SecondsStrokeProperty, value);
    }
    public IImmutableBrush MinutesStroke
    {
        get => GetValue(MinutesStrokeProperty);
        set => SetValue(MinutesStrokeProperty, value);
    }
    public IImmutableBrush HoursStroke
    {
        get => GetValue(HoursStrokeProperty); 
        set => SetValue(HoursStrokeProperty, value);
    }
    public IImmutableBrush SecondsHand
    {
        get => GetValue(SecondsHandProperty);
        set => SetValue(SecondsHandProperty, value);
    }
    public IImmutableBrush MinutesHand
    {
        get => GetValue(MinutesHandProperty);
        set => SetValue(MinutesHandProperty, value);
    }
    public IImmutableBrush HoursHand
    {
        get => GetValue(HoursHandProperty); 
        set => SetValue(HoursHandProperty, value);
    }
    public IImmutableBrush Hover
    {
        get => GetValue(HoverProperty);
        set => SetValue(HoverProperty, value);
    }
    public bool SecondsVisibility
    {
        get => GetValue(SecondsVisibilityProperty);
        set => SetValue(SecondsVisibilityProperty, value);
    }
    public bool MinutesVisibility
    {
        get => GetValue(MinutesVisibilityProperty);
        set => SetValue(MinutesVisibilityProperty, value);
    }
    public bool HoursVisibility
    {
        get => GetValue(HoursVisibilityProperty);
        set => SetValue(HoursVisibilityProperty, value);
    }
    public bool WatchMode
    {
        get => GetValue(WatchModeProperty);
        set => SetValue(WatchModeProperty, value);
    }
    public double Seconds
    {
        get => GetValue(SecondsProperty);
        set => SetValue(SecondsProperty, value);
    }
    public double Minutes
    {
        get => GetValue(MinutesProperty);
        set => SetValue(MinutesProperty, value);
    }
    public double Hours
    {
        get => GetValue(HoursProperty);
        set => SetValue(HoursProperty, value);
    }

    private int diff;
    private bool arranged;
    private double radius;
    private double xoffset;
    private double yoffset;
    private bool hourslocked;
    private bool minuteslocked;
    private bool secondslocked;
    private double circelradius;
    private double secondsradius;
    private double minutesradius;
    private double hourspmradius;
    private double hoursamradius;
    private IndicatorHelper hoursindicatorhelper;
    private IndicatorHelper secondsindicatorhelper;
    private IndicatorHelper minutesindicatorhelper;
    static ArcTimerControl()
    {
        SecondsProperty.Changed.AddClassHandler<ArcTimerControl>(HandleSecondsChanged);
        MinutesProperty.Changed.AddClassHandler<ArcTimerControl>(HandleMinutesChanged);
        HoursProperty.Changed.AddClassHandler<ArcTimerControl>(HandleHoursChanged);
    }

    private static void HandleSecondsChanged(ArcTimerControl control, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.NewValue is double value)
            control.HandleSeconds(value);
    }
    private static void HandleMinutesChanged(ArcTimerControl control, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.NewValue is double value)
            control.HandleMinutes(value);
    }
    private static void HandleHoursChanged(ArcTimerControl control, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.NewValue is double value)
            control.HandleHours(value, value <= 12);
    }
    public ArcTimerControl()
    {
        InitializeComponent();
    }
    private void grid_SizeChanged(object sender, SizeChangedEventArgs e) => Arrange();
    private void Arrange()
    {
        diff = 12;
        Rect bounds = contentcanvas.Bounds;

        radius = Math.Min(bounds.Width, bounds.Height) / 2;
        xoffset = bounds.Width / 2;
        yoffset = bounds.Height / 2;
        secondsradius = radius - diff;
        minutesradius = secondsradius - diff;
        hoursamradius = minutesradius - diff;
        hourspmradius = hoursamradius - diff;
        circelradius = hourspmradius - 9;

        if (circelradius < 0)
            return;

        seconds.Data = CreateGeometry(xoffset, yoffset, secondsradius, 60);
        minutes.Data = CreateGeometry(xoffset, yoffset, minutesradius, 60);
        hourspm.Data = CreateGeometry(xoffset, yoffset, hourspmradius, 12);
        hoursam.Data = CreateGeometry(xoffset, yoffset, hoursamradius, 12);

        double hoursradius = Hours > 12 ? hourspmradius : hourspmradius;
        secondsindicator.Data = GenerateIndicator(xoffset, yoffset, secondsradius, 60, (int)Seconds, out secondsindicatorhelper);
        minutesindicator.Data = GenerateIndicator(xoffset, yoffset, minutesradius, 60, (int)Minutes, out minutesindicatorhelper);
        hourspmindicator.Data = GenerateIndicator(xoffset, yoffset, hoursradius, 12, (int)Hours, out hoursindicatorhelper);

        SetCircelPosition(clockcircle, bounds.Width, bounds.Height, circelradius, 0, 0);
        SetCircelPosition(middledot, bounds.Width, bounds.Height, 5, 0, 0);

        SetLinePosition(secondsline, bounds.Width, bounds.Height, circelradius, 1.00, 2 * Math.PI / 60 * Seconds);
        SetLinePosition(minutesline, bounds.Width, bounds.Height, circelradius, 0.85, 2 * Math.PI / 60 * Minutes);
        SetLinePosition(hoursline, bounds.Width, bounds.Height, circelradius, 0.7, 2 * Math.PI / 12 * Hours);

        arranged = true;

        HandleSeconds(Seconds);
        HandleMinutes(Minutes);
        HandleHours(Hours, Hours >= 12);
    }
    private ArcSegment CreateArc(Point point, double radius, double phi, SweepDirection direction, bool islarge)
    {
        return new ArcSegment()
        {
            Size = new Size(radius, radius),
            SweepDirection = direction,
            IsLargeArc = islarge,
            RotationAngle = phi,
            Point = point,
        };
    }

    private Point CreatePoint(double x, double y, double radius, double phi) => new Point(x + radius * Math.Cos(-phi + 0.5 * Math.PI), y - radius * Math.Sin(phi + 0.5 * Math.PI));
    private PathGeometry CreateGeometry(double xcenter, double ycenter, double radius, int elements)
    {
        PathGeometry pathgeometry = new PathGeometry();
        double arcspan = 2 * Math.PI / elements;
        double arcwidth = (arcspan - 0.02) / 2;

        for (int i = 0; i < elements; ++i)
        {
            Point startpointouter = CreatePoint(xcenter, ycenter, radius, i * arcspan - arcwidth);
            Point endpointouter = CreatePoint(xcenter, ycenter, radius, i * arcspan + arcwidth);

            PathFigure figure = GeneratePathFigure(startpointouter, endpointouter, radius, 2 * arcwidth, out _);
            pathgeometry.Figures.Add(figure);
        }

        return pathgeometry;
    }
    private PathGeometry GenerateIndicator(double xcenter, double ycenter, double radius, int elements, int position, out IndicatorHelper indicator)
    {
        double arcspan = 2 * Math.PI / elements;
        double arcwidth = (arcspan - 0.02) / 2;
        Point startpointouter = CreatePoint(xcenter, ycenter, radius, position * arcspan - arcwidth);
        Point endpointouter = CreatePoint(xcenter, ycenter, radius, position * arcspan + arcwidth);

        PathFigure figure = GeneratePathFigure(startpointouter, endpointouter, radius, 2 * arcwidth, out ArcSegment segment);
        PathGeometry pathgeometry = new PathGeometry();
        pathgeometry.Figures.Add(figure);

        indicator = new IndicatorHelper(figure, segment);
        return pathgeometry;
    }
    private PathFigure GeneratePathFigure(Point startpoint, Point endpoint, double radius, double arcspan, out ArcSegment segment)
    {
        segment = CreateArc(endpoint, radius, arcspan, SweepDirection.Clockwise, false);
        PathFigure figure = new PathFigure()
        {
            StartPoint = startpoint,
            Segments = { segment },
        };
        return figure;
    }
    private void SetCircelPosition(Ellipse ellipse, double width, double height, double circelradius, double centerradius, double phi)
    {
        if (width < 0)
            return;
        if (height < 0)
            return;
        if (circelradius < 0)
            return;
        if (centerradius < 0)
            return;


        ellipse.Width = circelradius * 2;
        ellipse.Height = circelradius * 2;
    }
    private void SetLinePosition(Line line, double width, double height, double radius, double scale, double phi)
    {
        if (width < 0)
            return;
        if (height < 0)
            return;
        if (radius < 0)
            return;

        line.StartPoint = new Point(width / 2.0, height / 2.0);
        line.EndPoint = new Point(width / 2.0 + scale * radius * Math.Cos(-phi + 0.5 * Math.PI), height / 2.0 - scale * radius * Math.Sin(-phi + 0.5 * Math.PI));
    }
    private void PathMouseDonw(object sender, PointerPressedEventArgs e)
    {
        if (WatchMode)
            return;

        Rect bounds = contentcanvas.Bounds;
        PointerPoint position = e.GetCurrentPoint(contentcanvas);
        double yoffset = grid.Bounds.Height - bounds.Height;
        double phi = CalcPhi(new Point(position.Position.X, position.Position.Y - yoffset), bounds.Width / 2, bounds.Height / 2);

        if (Equals(sender, seconds))
        {
            int factor = CalculateValue(phi, 60);
            HandleSeconds(factor);
        }
        if (Equals(sender, minutes))
        {
            int factor = CalculateValue(phi, 60);
            HandleMinutes(factor);
        }
        if (Equals(sender, hourspm))
        {
            int factor = CalculateValue(phi, 12);
            HandleHours(factor, false);
        }
        if (Equals(sender, hoursam))
        {
            int factor = CalculateValue(phi, 12);
            HandleHours(factor, true);
        }
    }

    private double CalcPhi(Point position, double xcenter, double ycenter)
    {
        double y = ycenter - position.Y;
        double x = position.X - xcenter;

        if (y < 0 && x < 0)
        {
            return Math.Atan(-x / -y) + Math.PI;
        }
        if (y < 0)
        {
            return Math.Atan(-y / x) + 0.5 * Math.PI;
        }
        if (x < 0)
        {
            return Math.Atan(y / -x) + 1.5 * Math.PI;
        }

        return Math.Atan(x / y);
    }
    private int CalculateValue(double phi, int elements)
    {
        double arcphi = 2 * Math.PI / elements;
        double arcgap = arcphi * 0.45;

        double factor = Round(phi / arcphi);
        double diff = phi - factor * arcphi;

        if (phi > (arcphi * factor - arcgap) && phi < (arcphi * factor + arcgap))
        {

        }

        return (int)factor % elements;
    }
    private double Round(double value)
    {
        double temp = Math.Abs(value);

        if ((temp - (int)temp) > 0.5)
            return Math.Sign(value) * ((int)temp + 1);

        return (int)temp;
    }
    private void HandleSeconds(double value)
    {
        if (arranged == false)
            return;

        if (secondslocked)
            return;

        Rect bounds = contentcanvas.Bounds;

        secondslocked = true;   
        Seconds = Math.Max(Math.Min(value, 59), 0);
        secondslocked = false;

        SetIndicator(secondsindicatorhelper, secondsradius, 60, (int)value);
        SetLinePosition(secondsline, bounds.Width, bounds.Height, circelradius, 1, 2 * Math.PI / 60 * value);
    }
    private void HandleMinutes(double value)
    {
        if (arranged == false)
            return;

        if (minuteslocked)
            return;

        Rect bounds = contentcanvas.Bounds;

        minuteslocked = true;
        Minutes = Math.Max(Math.Min(value, 59), 0);
        minuteslocked = false;

        SetIndicator(minutesindicatorhelper, minutesradius, 60, (int)value);
        SetLinePosition(minutesline, bounds.Width, bounds.Height, circelradius, 0.85, 2 * Math.PI / 60 * value);
    }
    private void HandleHours(double value, bool am)
    {
        if (arranged == false)
            return;

        if (hourslocked)
            return;

        double hoursradius = am ? hoursamradius : hourspmradius;
        value = am && value < 12 ? value += 12 : value;

        hourslocked = true;
        Hours = Math.Max(Math.Min(am ? value % 24 : value % 12, 23), 0);
        hourslocked = false;

        Rect bounds = contentcanvas.Bounds;
        SetIndicator(hoursindicatorhelper, hoursradius, 12, (int)value);
        SetLinePosition(hoursline, bounds.Width, bounds.Height, circelradius, 0.7, 2 * Math.PI / 12 * value);
    }

    private void SetIndicator(IndicatorHelper indicator, double radius, int segments, int element)
    {
        double acrspan = 2 * Math.PI / segments;
        double arcwidth = (acrspan - 0.02) / 2;
        double startangle = element * acrspan - arcwidth;
        double endangle = element * acrspan + arcwidth;

        Point startpoint = CreatePoint(xoffset, yoffset, radius, startangle);
        Point endpoint = CreatePoint(xoffset, yoffset, radius, endangle);

        indicator.SetPosition(startpoint, endpoint, radius, 2 * arcwidth);
    }

    private void PathMouseWheel(object sender, PointerWheelEventArgs e)
    {
        if (sender is Avalonia.Controls.Shapes.Path path)
        {
            if (Equals(path, seconds))
            {
                int value = (int)Seconds + Math.Sign(e.Delta.Y);
                Seconds = (value < 0 ? 60 + value : value) % 60;
            }
            if (Equals(path, minutes))
            {
                int value = (int)Minutes + Math.Sign(e.Delta.Y);
                Minutes = (value < 0 ? 60 + value : value) % 60;
            }
            if (Equals(path, hourspm) || Equals(path, hoursam))
            {
                int value = (int)Hours + Math.Sign(e.Delta.Y);
                Hours = (value < 0 ? 24 + value : value) % 24;
            }
        }
    }
}