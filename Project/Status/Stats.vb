Public Class Stats
    Private p_game As Game
    Private p_font As Font
    Private p_font2 As Font
    Private p_position As PointF
    Private p_visible As Boolean
    Private p_bg As Bitmap
    Public Sub New(ByRef game As Game)
        p_game = game
        p_bg = game.LoadBitmap("gui_bg.png")
        p_font = New Font("Narkisim", 24, FontStyle.Bold, GraphicsUnit.Pixel)
        p_font2 = New Font("Narkisim", 14, FontStyle.Regular, GraphicsUnit.Pixel)
        p_visible = False
        p_position.X = (Form1.Width / 2) - (p_bg.Width / 2)
        p_position.Y = (Form1.Height / 2) - (p_bg.Height / 2)
    End Sub
    Public Property Visible() As Boolean
        Get
            Return p_visible
        End Get
        Set(ByVal value As Boolean)
            p_visible = value
        End Set
    End Property
    Public Property Position() As PointF
        Get
            Return p_position
        End Get
        Set(ByVal value As PointF)
            p_position = value
        End Set
    End Property
    Private Sub Print(ByVal x As Integer, ByVal y As Integer, ByVal text As String)
        Print(x, y, text, Brushes.White)
    End Sub
    Private Sub Print(ByVal x As Integer, ByVal y As Integer, ByVal text As String, ByVal color As Brush)
        p_game.Device.DrawString(text, p_font, color, x, y)
    End Sub
    Private Sub PrintRight(ByVal x As Integer, ByVal y As Integer, ByVal text As String, ByVal color As Brush)
        Dim rsize As SizeF
        rsize = p_game.Device.MeasureString(text, p_font)
        p_game.Device.DrawString(text, p_font, color, x - rsize.Width, y)
    End Sub
    Public Sub Draw()
        If Not p_visible Then Return

        REM draw background 
        p_game.DrawBitmap(p_bg, p_position.X, p_position.Y)

        Dim green As New Pen(Color.FromArgb(255, 76, 255, 0), 1)

        REM print player stats
        Dim x As Integer = p_position.X + 26
        Dim y As Integer = p_position.Y + 26
        Dim ht As Integer = 26
        Dim statX As Integer = 660 - (400 - x)
        Print(x, y, p_game.hero.Name, Brushes.Gold) : y += ht + 8
        PrintRight(statX, y, p_game.hero.Level.ToString(), Brushes.LightGreen)
        Print(x, y, "Level", green.Brush) : y += ht
        PrintRight(statX, y, p_game.hero.Experience.ToString(), Brushes.LightBlue)
        Print(x, y, "Experience", Brushes.LightBlue) : y += ht + 8
        PrintRight(statX, y, p_game.hero.STR.ToString(), Brushes.LightGreen)
        Print(x, y, "Strength", green.Brush) : y += ht
        PrintRight(statX, y, p_game.hero.DEX.ToString(), Brushes.LightBlue)
        Print(x, y, "Dexterity", Brushes.LightBlue) : y += ht
        PrintRight(statX, y, p_game.hero.STA.ToString(), Brushes.LightGreen)
        Print(x, y, "Stamina", green.Brush) : y += ht
        PrintRight(statX, y, p_game.hero.INT.ToString(), Brushes.LightBlue)
        Print(x, y, "Intellect", Brushes.LightBlue) : y += ht
        PrintRight(statX, y, p_game.hero.CHA.ToString(), Brushes.LightGreen)
        Print(x, y, "Charisma", green.Brush) : y += ht + 8
        PrintRight(statX, y, p_game.hero.Gold.ToString(), Brushes.Yellow)
        Print(x, y, "Gold", Brushes.LightGoldenrodYellow) : y += ht

        p_game.Update()
    End Sub
End Class