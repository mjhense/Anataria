Module StatusIconManager
    Private game As Game
    Private healthBar As Bitmap
    Private manaBar As Bitmap
    Private healthBarBG As Bitmap
    Private font As Font = New Font("Narkisim", 14)
    Public ReadOnly Property healthGaugue As Bitmap
        Get
            Return healthBar
        End Get
    End Property
    Public ReadOnly Property healthBGBar As Bitmap
        Get
            Return healthBarBG
        End Get
    End Property
    Public Sub Initialize(ByRef game1 As Game)
        game = game1
        healthBar = game.LoadBitmap("health.png")
        manaBar = game.LoadBitmap("mana.png")
        healthBarBG = game.LoadBitmap("healthOutline.png")
    End Sub
    Public Sub DrawStats()
        Dim x As Integer = 20
        Dim y As Integer = 12
        Dim sz As SizeF = game.Device.MeasureString(game.hero.Name, font)
        game.Device.DrawString(game.hero.Name, font, Brushes.Yellow, x, y) : y += sz.Height + 12
        Dim percent As Single
        percent = game.hero.Health / game.hero.HitPoints
        sz = game.Device.MeasureString("HP ", font)
        game.Device.DrawString("HP ", font, Brushes.White, x, y) : x += sz.Width + 2
        game.Device.DrawImageUnscaled(healthBarBG, x, y)
        game.Device.DrawImage(healthBar, x + 1, y + 1, healthBar.Width * CSng(FormatNumber(percent, 4)), healthBar.Height)
    End Sub
End Module