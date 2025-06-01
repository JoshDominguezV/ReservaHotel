using System.Drawing.Drawing2D;

public class RotatingImageButton : UserControl
{
    private Image _image;
    private float _rotationAngle = 0f;
    private bool _mouseOver = false;

    public Image Image
    {
        get => _image;
        set
        {
            _image = value;
            Invalidate(); // Redibuja el control
        }
    }

    public float RotationAngle
    {
        get => _rotationAngle;
        set
        {
            _rotationAngle = value;
            Invalidate(); // Redibuja el control
        }
    }

    // Propiedad pública para saber si el mouse está encima
    public bool MouseIsOver => _mouseOver;

    public RotatingImageButton()
    {
        this.Size = new Size(32, 32);


        // Optimiza el dibujo para evitar parpadeos
        this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                      ControlStyles.OptimizedDoubleBuffer |
                      ControlStyles.ResizeRedraw |
                      ControlStyles.SupportsTransparentBackColor |
                      ControlStyles.UserPaint, true);

        this.Cursor = Cursors.Hand;

        this.MouseEnter += (s, e) =>
        {
            _mouseOver = true;
            Invalidate();
        };
        this.MouseLeave += (s, e) =>
        {
            _mouseOver = false;
            Invalidate();
        };
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        e.Graphics.Clear(Color.Transparent);

        if (_image == null)
            return;

        PointF center = new PointF(this.Width / 2f, this.Height / 2f);

        var state = e.Graphics.Save();

        e.Graphics.TranslateTransform(center.X, center.Y);
        e.Graphics.RotateTransform(_rotationAngle);
        e.Graphics.TranslateTransform(-_image.Width / 2f, -_image.Height / 2f);

        int x = (this.Width - _image.Width) / 2;
        int y = (this.Height - _image.Height) / 2;
        e.Graphics.DrawImage(_image, x, y);


        e.Graphics.Restore(state);

        if (_mouseOver)
        {
            using (Pen p = new Pen(Color.FromArgb(100, Color.Black), 2))
            {
                e.Graphics.DrawEllipse(p, 1, 1, this.Width - 3, this.Height - 3);
            }
        }
    }
}
