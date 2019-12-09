Public Class Game
    Public Const savegamefile As String = "savegamefile.save"
    Public Structure DrawableItem
        Public item As Item
        Public sprite As Sprite
    End Structure
    Public Structure KeyStates
        Public up, down, left, right, shift, t As Boolean
    End Structure
    Public Enum GameStates
        STATE_TITLE
        STATE_CHARACTER
        STATE_PLAYING
        STATE_PAUSED
        STATE_STATS
    End Enum
    Public ReadOnly Property ScreenRectangle() As Rectangle
        Get
            Return New Rectangle(world.ScrollPos.X, world.ScrollPos.Y, 800, 600)
        End Get
    End Property
    Private p_device As Graphics
    Public p_surface As Bitmap
    Public p_frm As Form1
    Private p_font As Font
    Private p_gameOver As Boolean
    Private p_random As Random
    Private WithEvents p_pb As PictureBox
    Private p_mousePos As Point
    Private p_mouseBtn As MouseButtons
    Private p_scriptState As Integer
    Private p_scriptDialog As Dialogue
    Public ReadOnly Property Font As Font
        Get
            Return p_font
        End Get
    End Property
    Public p_dialog As Integer = 0
    Public casualVisible As Boolean = False
    Public keyState As KeyStates
    Public gameState As GameStates
    Public hero As Player
    Public titleScreen As Title
    Public inven As Inventory
    Public stats As Stats
    Public world As Level
    Public combat As Combat
    Public treasure As List(Of DrawableItem)
    Public NPCs As List(Of Character)
    Public heroProjectiles As New List(Of Character)
    Public monsterProjectiles As New List(Of Character)
    Public items As Items
    Public quests As Quests
    Public dialog As Dialogue
    Public charBuilder As CharacterBuilder
    Public tutorialKnight1 As Character
    Public tutorialKnight2 As Character
    Public Vendor As Character
    Public BlueKnight As Character
    Public GreenKnight As Character
    Public VladSoldier As Character
    Public GoodKing As Character
    Public VikingKing As Character
    Public PeasantAnna As Character
    Public Peasant As Character
    Public Urix As Character
    Public DarkKnight As Character
    Public ChestFlag As Boolean = False
    Public fadingTexts As New List(Of FadableText)
    Public doorFlag As Boolean = False
    Public animals As New List(Of Animal)
    Public eventMonsters As New List(Of Character)
    Public switchFlag As Boolean = False
    Public particles As New List(Of Particle)
    Protected Overrides Sub Finalize()
        REM free memory
        p_device.Dispose()
        p_surface.Dispose()
        p_pb.Dispose()
        p_font.Dispose()
    End Sub
    Public Sub New(ByRef form As Form1, ByVal width As Integer, ByVal height As Integer)
        p_device = Nothing
        p_surface = Nothing
        p_pb = Nothing
        p_frm = Nothing
        p_font = Nothing
        p_gameOver = False
        p_random = New Random()

        REM set form properties
        p_frm = form
        p_frm.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable
        p_frm.MaximizeBox = True
        REM adjust size for window border
        p_frm.Size = New Point(width, height)
        p_frm.Location = New Point((My.Computer.Screen.Bounds.Width / 2) - (p_frm.Width / 2), (My.Computer.Screen.WorkingArea.Height / 2) - (p_frm.Height / 2))
        p_frm.StartPosition = FormStartPosition.Manual
        p_frm.Text = "Anataria"

        REM create a picturebox
        p_pb = New PictureBox()
        p_pb.Parent = p_frm
        p_pb.Location = New Point(0, 0)
        p_pb.Size = New Size(width, height)
        p_pb.Dock = DockStyle.Fill
        p_pb.BackColor = Color.Black

        REM create graphics device
        p_surface = New Bitmap(p_frm.Size.Width, p_frm.Size.Height)
        p_pb.Image = p_surface
        p_device = Graphics.FromImage(p_surface)

        REM set graphic rending options
        '  p_device.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        REM set the default font
        SetFont("Narkisim", 14, FontStyle.Regular)

    End Sub
    Public ReadOnly Property Device() As Graphics
        Get
            If Threading.Monitor.TryEnter(p_device, 300) Then
                Try
                    Return p_device
                Finally
                    Threading.Monitor.Exit(p_device)
                End Try
            Else
                Return Nothing
            End If
        End Get
    End Property
    Public Sub Update()
        REM refresh the drawing surface
        p_pb.Image = p_surface
    End Sub
    Public Sub SetFont(ByVal name As String, ByVal size As Integer, ByVal style As FontStyle)
        p_font = New Font(name, size, style, GraphicsUnit.Pixel)
    End Sub
    Public Sub Print(ByVal x As Integer, ByVal y As Integer, ByVal text As String, ByVal color As Brush)
            p_device.DrawString(text, p_font, color, x, y)
    End Sub
    Public Sub Print(ByVal x As Integer, ByVal y As Integer, ByVal text As String)
        Print(x, y, text, Brushes.White)
    End Sub
    Public Sub Print(ByVal pos As Point, ByVal text As String, ByVal color As Brush)
        Print(pos.X, pos.Y, text, color)
    End Sub
    Public Sub Print(ByVal pos As Point, ByVal text As String)
        Print(pos.X, pos.Y, text)
    End Sub
    Public Function LoadBitmap(ByVal filename As String)
        Dim bmp As Bitmap
        Try
            bmp = New Bitmap(filename)
        Catch ex As Exception
            bmp = Nothing
        End Try
        Return bmp
    End Function
    Public Sub DrawBitmap(ByRef bmp As Bitmap, ByVal x As Single, ByVal y As Single)
        p_device.DrawImageUnscaled(bmp, x, y)
    End Sub
    Public Sub DrawBitmap(ByRef bmp As Bitmap, ByVal x As Single, ByVal y As Single, ByVal width As Integer, ByVal height As Integer)
        p_device.DrawImageUnscaled(bmp, x, y, width, height)
    End Sub
    Public Sub DrawBitmap(ByRef bmp As Bitmap, ByVal pos As Point)
        p_device.DrawImageUnscaled(bmp, pos)
    End Sub
    Public Sub DrawBitmap(ByRef bmp As Bitmap, ByVal pos As Point, ByVal size As Size)
        p_device.DrawImageUnscaled(bmp, pos.X, pos.Y, size.Width, size.Height)
    End Sub
    Public Function FrameRate() As Integer
        Static count As Integer = 0
        Static lastTime As Integer = 0
        Static frames As Integer = 0

        REM calculate core frame rate
        Dim ticks As Integer = My.Computer.Clock.TickCount()
        count += 1
        If ticks > lastTime + 1000 Then
            lastTime = ticks
            frames = count
            count = 0
        End If
        Return frames
    End Function
    Public Function Random(ByVal max As Integer)
        Return Random(0, max)
    End Function
    Public Function Random(ByVal min As Integer, ByVal max As Integer)
        Return p_random.Next(min, max + 1)
    End Function
    Public Function Distance(ByVal first As PointF, ByVal second As PointF) As Single
        Dim deltaX As Single = second.X - first.X
        Dim deltaY As Single = second.Y - first.Y
        Dim dist = Math.Sqrt(deltaX * deltaX + deltaY * deltaY)
        Return dist
    End Function
    Public Function Distance(ByVal x1 As Single, ByVal y1 As Single, _
            ByVal x2 As Single, ByVal y2 As Single) As Single
        Dim first As New PointF(x1, y1)
        Dim second As New PointF(x2, y2)
        Return Distance(first, second)
    End Function
    Private Sub p_pb_MouseInput(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
    Handles p_pb.MouseMove, p_pb.MouseDown
        p_mousePos.X = e.X
        p_mousePos.Y = e.Y
        p_mouseBtn = e.Button
    End Sub
    Public Property MousePos() As Point
        Get
            Return p_mousePos
        End Get
        Set(ByVal value As Point)
            p_mousePos = value
        End Set
    End Property
    Public Property MouseButton() As MouseButtons
        Get
            Return p_mouseBtn
        End Get
        Set(ByVal value As MouseButtons)
            p_mouseBtn = value
        End Set
    End Property
    Public Sub showDialog(ByVal title As String, ByVal message As String, ByVal button1 As String)
        dialog.Title = title
        dialog.Message = message
        dialog.NumButtons = 1
        dialog.setButtonText(1, button1)
        dialog.setCorner(Dialogue.Positions.UpperLeft)
        dialog.Visible = True
        dialog.Selection = -1
    End Sub
    Public Sub showDialog(ByVal title As String, ByVal message As String, ByVal button1 As String, ByVal button2 As String)
        dialog.Title = title
        dialog.Message = message
        dialog.NumButtons = 2
        dialog.setButtonText(1, button1)
        dialog.setButtonText(2, button2)
        dialog.setCorner(Dialogue.Positions.UpperLeft)
        dialog.Visible = True
        dialog.Selection = -1
    End Sub
    Public Sub showDialogTutorialKnight1(ByVal title As String, ByVal message As String)
        dialog.Title = title
        dialog.Message = message
        dialog.NumButtons = 0
        dialog.setCorner(Dialogue.Positions.UpperLeft)
        dialog.Visible = True
        dialog.Selection = 1
    End Sub
    Public Sub showDialogNoButtons(ByVal title As String, ByVal message As String)
        dialog.Title = title
        dialog.Message = message
        dialog.setCorner(Dialogue.Positions.UpperLeft)
        dialog.Visible = True
    End Sub
    Public Sub CasualDialog(ByVal character As Character)
        If casualVisible = False Then
            casualVisible = True
        End If
        If casualVisible = True Then
            Select Case p_dialog
                Case 0
                    Me.showDialogTutorialKnight1(character.Name, "Hello there.")
                    Exit Select
                Case 1
                    Me.showDialogTutorialKnight1(character.Name, "Lovely weather we have today.")
                    Exit Select
                Case 2
                    Me.showDialogTutorialKnight1(character.Name, "How's it going?")
                    Exit Select
                Case 3
                    Me.showDialogTutorialKnight1(character.Name, "I like you.")
                Case 4
                    Me.showDialogTutorialKnight1(character.Name, "....")
                    Exit Select
            End Select
        End If
    End Sub
    Public Sub showShopDialog(ByRef dialog As Dialogue, ByVal title As String, ByVal message As String, btnNum As Integer, Optional btn1 As String = "", Optional btn2 As String = "", Optional btn3 As String = "", _
        Optional btn4 As String = "", Optional btn5 As String = "", Optional btn6 As String = "", Optional btn7 As String = "", Optional btn8 As String = "", Optional btn9 As String = "", _
        Optional btn10 As String = "", Optional v1 As Integer = 0, Optional v2 As Integer = 0, Optional v3 As Integer = 0, Optional v4 As Integer = 0, Optional v5 As Integer = 0, Optional v6 As Integer = 0, _
        Optional v7 As Integer = 0, Optional v8 As Integer = 0, Optional v9 As Integer = 0, Optional v10 As Integer = 0)
        dialog.Visible = True
        dialog.Selection = 0
        dialog.Title = title
        dialog.Message = message
        dialog.NumButtons = btnNum
        dialog.setButtonText(1, btn1, v1)
        dialog.setButtonText(2, btn2, v2)
        dialog.setButtonText(3, btn3, v3)
        dialog.setButtonText(4, btn4, v4)
        dialog.setButtonText(5, btn5, v5)
        dialog.setButtonText(6, btn6, v6)
        dialog.setButtonText(7, btn7, v7)
        dialog.setButtonText(8, btn8, v8)
        dialog.setButtonText(9, btn9, v9)
        dialog.setButtonText(10, btn10, v10)
    End Sub
    Public Sub LuaWrite(ByVal x As Integer, ByVal y As Integer, ByVal text As String)
        Print(x, y, text, Brushes.White)
    End Sub
    Public Sub LuaLoadLevel(ByVal filename As String)
        If Not world.loadTilemap(filename) Then
            MessageBox.Show("Error loading tilemap file: " + filename)
            End
        End If
    End Sub
    Public Sub LuaLoadItems(ByVal filename As String)
        If Not items.Load(filename) Then
            MessageBox.Show("Error loading items file: " + filename)
            End
        End If
    End Sub
    Public Sub LuaLoadHero(ByVal filename As String)
        If Not hero.Load(filename) Then
            MessageBox.Show("Error loading hero character file: " + filename)
            End
        End If
    End Sub
    Public Sub LuaLoadQuests(ByVal filename As String)
        If Not quests.Load(filename) Then
            MessageBox.Show("Error loading quests file: " + filename)
            End
        End If
    End Sub
    Public Sub LuaMessage(ByVal title As String, ByVal message As String, ByVal button1 As String)
        p_scriptDialog.Title = title
        p_scriptDialog.Message = message
        p_scriptDialog.NumButtons = 1
        p_scriptDialog.setButtonText(1, button1)
        p_scriptDialog.setCorner(Dialogue.Positions.UpperLeft)
        p_scriptDialog.Visible = True
        p_scriptDialog.Selection = -1
    End Sub
    Public Sub LuaDropGold(ByVal amount As Integer, ByVal x As Integer, ByVal y As Integer)
        combat.DropGold(amount, x, y)
    End Sub
    Public Sub LuaDropItem(ByVal itemname As String, ByVal x As Integer, ByVal y As Integer)
        Dim itm As Item = items.GetItem(itemname)
        If itm IsNot Nothing Then
            combat.DropTreasureItem(itm, x, y)
        Else
            MessageBox.Show("Error: '" + itemname + "' is not a valid item")
            End
        End If
    End Sub
    Public Sub AddCharacter(ByVal filename As String, ByVal x As Integer, ByVal y As Integer)
        Dim range As New Rectangle(x, y, 0, 0)
        AddCharacter(filename, range)
    End Sub
    Public Sub AddCharacter(ByVal filename As String, ByVal range As Rectangle)
        LuaAddCharacter(filename, range.X, range.Y, range.Width, range.Height)
    End Sub
    Public Sub LuaAddCharacter(ByVal filename As String, ByVal x As Integer, ByVal y As Integer, ByVal rangex As Integer, ByVal rangey As Integer)
        Dim monster As New Character(Me)
        monster.Load(filename)
        monster.range = New Rectangle(x, y, rangex, rangey)
        Dim pos As New Point(x, y)
        pos.X += Random(rangex)
        pos.Y += Random(rangey)
        monster.Position = pos
        NPCs.Add(monster)
    End Sub
    Public Function LuaMakeCharacter(ByVal filename As String, x As Integer, y As Integer, rangex As Integer, rangey As Integer) As Character
        Dim monster As New Character(Me)
        monster.Load(filename)
        monster.range = New Rectangle(x, y, rangex, rangey)
        Dim pos As New Point(x, y)
        monster.Position = pos
        Return monster
    End Function
    Public Sub LuaAddCharacter(chartr As Character)
        NPCs.Add(chartr)
    End Sub
    Public Function ReturnFireSprite(game As Game, pos As PointF) As Sprite
        Dim bmp As Bitmap
        Dim sprite As Sprite
        Dim relativePos As PointF
        bmp = Me.LoadBitmap("fire.png")
        sprite = New Sprite(game)
        sprite.Image = bmp
        sprite.Size = New Size(32, 32)
        sprite.Columns = 20
        sprite.CurrentFrame = 0
        sprite.Alive = True
        relativePos = New Point(pos.X, pos.Y)
        Return sprite
    End Function
    Public Function ReturnIceSprite(game As Game, pos As PointF) As Sprite
        Dim bmp As Bitmap
        Dim sprite As Sprite
        Dim relativePos As PointF
        bmp = Me.LoadBitmap("dizzy.png")
        sprite = New Sprite(game)
        sprite.Image = bmp
        sprite.Size = New Size(48, 48)
        sprite.TotalFrames = 36
        sprite.Columns = 6
        sprite.CurrentFrame = 0
        sprite.Alive = True
        relativePos = New Point(pos.X, pos.Y)
        Return sprite
    End Function
    Public Function ReturnPlatformSprite(game As Game, pos As PointF) As Sprite
        Dim bmp As Bitmap
        Dim relativePos As PointF
        Dim sprite As New Sprite(game)
        bmp = Me.LoadBitmap("HealthCircle.png")
        sprite.Image = bmp
        sprite.Size = bmp.Size
        sprite.Columns = 1
        sprite.CurrentFrame = 0
        sprite.Alive = True
        relativePos = New Point(pos.X, pos.Y)
        Return sprite
    End Function
    Public Sub AddFadingText(x As Single, y As Single, text As String, duration As Single, color As Color, font As Font, followPlayer As Boolean, direction As PointF)
        Dim f As FadableText = New FadableText(Me, text, x, y, duration, color, font, direction)
        f.followPlayer = followPlayer
        fadingTexts.Add(f)
    End Sub
    Public Sub AddFadingText(x As Single, y As Single, text As String, color As Color, duration As Single, direction As PointF)
        fadingTexts.Add(New FadableText(Me, text, x, y, duration, color, p_font, direction))
    End Sub
    Public Sub AddFadingText(x As Single, y As Single, text As String, duration As Single, direction As PointF)
        fadingTexts.Add(New FadableText(Me, text, x, y, duration, Color.White, p_font, direction))
    End Sub
    Public Sub AddFadingText(pos As PointF, text As String, duration As Single, color As Color, font As Font, followPlayer As Boolean, direction As PointF)
        Dim f As New FadableText(Me, text, pos, duration, color, font, direction)
        f.followPlayer = followPlayer
        fadingTexts.Add(f)
    End Sub
    Public Sub AddAnimationStrip(rect As Rectangle, filename As String, width As Integer, height As Integer, columns As Integer, frames As Integer, animationRate As Single)
        Dim sprite As New Particle(Me)
        sprite.Rect = rect
        sprite.Image = LoadBitmap(filename)
        sprite.Size = New Size(width, height)
        sprite.Columns = columns
        sprite.TotalFrames = frames
        sprite.X = rect.X
        sprite.Y = rect.Y
        sprite.AnimationRate = animationRate
        particles.Add(sprite)
    End Sub
    Public Function ToScreen(pos As Point) As Point
        If pos.X - world.X < 0 Or pos.Y - world.Y < 0 Then
            Return pos
        Else
            Return New Point(pos.X - world.X, pos.Y - world.Y)
        End If
    End Function
    Public Function ToScreen(pos As PointF) As PointF
        If pos.X - world.X < 0 Or pos.Y - world.Y < 0 Then
            Return pos
        Else
            Return New PointF(pos.X - world.X, pos.Y - world.Y)
        End If
    End Function
    Public Function ToScreen(rect As Rectangle) As Rectangle
        If rect.X - world.X < 0 Or rect.Y - world.Y < 0 Then
            Return rect
        Else
            Return New Rectangle(rect.X - world.X, rect.Y - world.Y, rect.Width, rect.Height)
        End If
    End Function
End Class