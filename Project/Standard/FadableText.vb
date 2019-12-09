Public Class FadableText
    Private _text As String
    Private _duration As Single
    Private totalDuration As Single
    Private _expired As Boolean
    Private _color As Color
    Private font As Font
    Private game As Game
    Private _pos As PointF
    Private velocity As PointF
    Private traveledDistance As New PointF(0, 0)
    Public followPlayer As Boolean
    Private ReadOnly Property DrawPos As PointF
        Get
            Return New PointF(traveledDistance.X + _pos.X, traveledDistance.Y + _pos.Y)
        End Get
    End Property
    Public Property Pos As PointF
        Get
            Return _pos
        End Get
        Set(value As PointF)
            _pos = value
        End Set
    End Property
    Public Property X As Single
        Get
            Return _pos.X
        End Get
        Set(value As Single)
            _pos.X = value
        End Set
    End Property
    Public Property Y As Single
        Get
            Return _pos.Y
        End Get
        Set(value As Single)
            _pos.Y = value
        End Set
    End Property
    Public Property Text As String
        Get
            Return _text
        End Get
        Set(value As String)
            _text = value
        End Set
    End Property
    Public ReadOnly Property Duration As Single
        Get
            Return _duration
        End Get
    End Property
    Public ReadOnly Property FontColor As Color
        Get
            Return _color
        End Get
    End Property
    Public ReadOnly Property Expired As Boolean
        Get
            Return _expired
        End Get
    End Property
    Public Sub New(ByRef game As Game, text As String, pos As PointF, duration As Single, color As Color, font As Font, direction As PointF)
        _expired = False
        _color = color
        _text = text
        Me.game = game
        _pos = pos
        Me._duration = duration * 1000
        Me.font = font
        velocity = direction
        totalDuration = duration * 1000
    End Sub
    Public Sub New(ByRef game As Game, text As String, x As Single, y As Single, duration As Single, color As Color, font As Font, direction As PointF)
        _expired = False
        _color = color
        _text = text
        Me.game = game
        _pos.X = x
        _pos.Y = y
        Me._duration = duration * 1000
        Me.font = font
        velocity = direction
        totalDuration = duration * 1000
    End Sub
    Public Sub Draw()
        _duration -= 1000 / Form1.FrameRate
        If _duration <= 0.0F Then
            _expired = True
        Else
            _expired = False
        End If
        If Expired Then Return
        traveledDistance.X += velocity.X
        traveledDistance.Y += velocity.Y
        If followPlayer Then
            Dim sz As SizeF = game.Device.MeasureString(_text, font)
            Me.Pos = New PointF(game.hero.GetSprite.X + (game.hero.GetSprite().Width / 2) - (sz.Width / 2), game.hero.GetSprite.Y - 20)
        End If
        Dim drawColor As Brush = New Pen(Color.FromArgb((255 * CSng(FormatNumber((_duration / totalDuration), 5))), _color)).Brush
        game.Device.DrawString(_text, font, drawColor, DrawPos.X, DrawPos.Y)
    End Sub
End Class