Imports System.Xml
Public Class Level
#Region "Delarations"
    Public Structure tilemapStruct
        Public tilenum As Integer
        Public data1 As String
        Public data2 As String
        Public data3 As String
        Public data4 As String
        Public collidable As Boolean
        Public portal As Boolean
        Public portalx As Integer
        Public portaly As Integer
        Public portalfile As String
        Public bgTileNum As Integer
        Public animate As Boolean
        Public tileX As Integer
        Public tileY As Integer
        Public tempCollidable As Boolean
        Public tempCollidableForMonsters
        Public animationNum As Integer
        Public ReadOnly Property Bounds As Rectangle
            Get
                Return New Rectangle(tileX * 32, tileY * 32, 32, 32)
            End Get
        End Property
    End Structure
    Private p_game As Game
    Private p_mapSize As New Point(0, 0)
    Private p_windowSize As New Point(0, 0)
    Private p_tileSize As Integer
    Private p_bmpTiles As Bitmap
    Private p_columns As Integer
    Private p_bmpScrollBuffer As Bitmap
    Private p_gfxScrollBuffer As Graphics
    Private p_tilemap() As tilemapStruct
    Private p_scrollPos As New PointF(0, 0)
    Private p_subtile As New PointF(0, 0)
    Private p_oldScrollPos As New PointF(-1, -1)
    Private p_portalFlag As Boolean
    Private p_portalTarget As Point
    Private p_collidableFlag As Boolean
    Private p_currentTile As tilemapStruct
    Private p_currentLevel As String
    Private Loadbg As Sprite
    Private p_bmpTilesDark As Bitmap
    Private p_LastSpawn As New Point(0, 0)
    Private DrawableStructures As New List(Of DrawableStructure)
    Private animationScripts As New List(Of DrawableScript)
    Private oldHour As Integer
    Private portalMarker As Sprite
    Private treeImage As Bitmap
    Private trees As New List(Of Tree)
    Private chests As New List(Of Chest)
    Private doors As New List(Of Door)
    Private tileAnims As New List(Of Sprite)
    Private laserCannons As New List(Of LaserCannon)
    Private switches As New List(Of Switch)
    Private Sub AddAnimal(filename As String, tileX As Integer, tileY As Integer)
        Dim character As Animal
        character = New Animal(p_game)
        character.Load(filename)
        character.Position = New PointF(tileX * p_tileSize, tileY * p_tileSize)
        character.range = New Rectangle(character.Position.X, character.Position.Y, 1000, 1000)
        p_game.animals.Add(character)
    End Sub
    Private Sub AddChest(BigChest As Boolean, itmName As String, G As Integer, x As Single, y As Single)
        Dim chest As Chest = New Chest(p_game, BigChest, itmName, G, x, y)
        chests.Add(chest)
    End Sub
    Private Sub AddDoor(type As Integer, keyName As String, x As Single, y As Single)
        Dim door As Door = New Door(p_game, keyName, type, x, y)
        doors.Add(door)
    End Sub
    Private Structure Tree
        Public relativePos As PointF
        Public Const width As Integer = 128
        Public Const height As Integer = 128
        Public srcRect As Rectangle
        Public ReadOnly Property Bounds As Rectangle
            Get
                Return New Rectangle(relativePos.X, relativePos.Y, width, height)
            End Get
        End Property
    End Structure
    Private Structure DrawableStructure
        Public bmp As Bitmap
        Public sprite As Sprite
        Public relativePos As PointF
        Public width As Integer
        Public height As Integer
    End Structure
    Public Structure DrawableScript
        Public bmp As Bitmap
        Public sprite As Sprite
        Public relativePos As PointF
        Public width As Integer
        Public height As Integer
        Public scriptName As String
    End Structure
#End Region
#Region "Properties"
    Public Property LoadedRecently As Boolean
    Public Property CurrentTile() As tilemapStruct
        Get
            Return p_currentTile
        End Get
        Set(ByVal value As tilemapStruct)
            p_currentTile = value
        End Set
    End Property
    Public Property CurrentLevel As String
        Get
            Return p_currentLevel
        End Get
        Set(value As String)
            p_currentLevel = value
        End Set
    End Property
    Public Property CollidableFlag() As Boolean
        Get
            Return p_collidableFlag
        End Get
        Set(ByVal value As Boolean)
            p_collidableFlag = value
        End Set
    End Property
    Public Property PortalFlag() As Boolean
        Get
            Return p_portalFlag
        End Get
        Set(ByVal value As Boolean)
            p_portalFlag = value
        End Set
    End Property
    Public Property PortalTarget() As Point
        Get
            Return p_portalTarget
        End Get
        Set(ByVal value As Point)
            p_portalTarget = value
        End Set
    End Property
    Public Property LastSpawn As Point
        Get
            Return p_LastSpawn
        End Get
        Set(value As Point)
            p_LastSpawn = value
        End Set
    End Property
    Public Function GetTile(ByVal p As PointF) As tilemapStruct
        Return GetTile(p.Y * 128 + p.X)
    End Function
    Public Function GetTile(ByVal pixelx As Integer, ByVal pixely As Integer) As tilemapStruct
        Return GetTile(pixely * 128 + pixelx)
    End Function
    Public Function GetTile(ByVal index As Integer) As tilemapStruct
        If index > p_tilemap.Count - 1 Then index = p_tilemap.Count - 1
        If index < 0 Then index = 0
        Return p_tilemap(index)
    End Function
    Public Sub SetTempCollidable(ByVal p As PointF, c As Boolean)
        Try
            p_tilemap(p.Y * 128 + p.X).tempCollidable = c
        Catch
        End Try
    End Sub
    Public Sub SetCollidable(ByVal p As PointF, c As Boolean)
        Try
            p_tilemap(p.Y * 128 + p.X).collidable = c
        Catch
        End Try
    End Sub
    Public Sub SetTempCollidableForMonsters(ByVal p As PointF, c As Boolean)
        Try
            p_tilemap(p.Y * 128 + p.X).tempCollidableForMonsters = c
        Catch
        End Try
    End Sub
    Public Property GridPos() As Point
        Get
            Dim x As Integer = p_scrollPos.X / p_tileSize
            Dim y As Integer = p_scrollPos.Y / p_tileSize
            Return New Point(x, y)
        End Get
        Set(ByVal value As Point)
            Dim x As Single = value.X * p_tileSize
            Dim y As Single = value.Y * p_tileSize
            p_scrollPos = New PointF(x, y)
        End Set
    End Property
    Public Property ScrollPos() As PointF
        Get
            Return p_scrollPos
        End Get
        Set(ByVal value As PointF)
            REM save new scroll position
            p_scrollPos = value
        End Set
    End Property
    Public Property X() As Integer
        Get
            Return p_scrollPos.X
        End Get
        Set(ByVal value As Integer)
            p_scrollPos.X = value
        End Set
    End Property
    Public Property Y() As Integer
        Get
            Return p_scrollPos.Y
        End Get
        Set(ByVal value As Integer)
            p_scrollPos.Y = value
        End Set
    End Property
#End Region
#Region "Constructor"
    Public Sub New(ByRef gameObject As Game, ByVal width As Integer, ByVal height As Integer, ByVal tileSize As Integer)
        p_game = gameObject
        p_windowSize = New Point(width, height)
        p_tileSize = tileSize
        p_mapSize = New Point(width * tileSize, height * tileSize)
        REM create scroll buffer
        p_bmpScrollBuffer = New Bitmap(p_mapSize.X + p_tileSize, p_mapSize.Y + p_tileSize)
        p_gfxScrollBuffer = Graphics.FromImage(p_bmpScrollBuffer)
        REM create tilemap 
        ReDim p_tilemap(128 * 128)
        Loadbg = New Sprite(p_game)
        Dim LoadImage As Bitmap = p_game.LoadBitmap("Load.png")
        Loadbg.Image = LoadImage
        Loadbg.Size = LoadImage.Size
        'Loadbg.Position = New Point((p_game.p_frm.Width / 2) - (Loadbg.Width / 2), (p_game.p_frm.Height / 2) - (Loadbg.Height / 2))
        Loadbg.Position = New PointF(0.0F, 0.0F)
        portalMarker = New Sprite(p_game)
        portalMarker.TotalFrames = 8
        portalMarker.Size = New Size(32, 32)
        portalMarker.Columns = 8
        portalMarker.CurrentFrame = 0
        portalMarker.AnimationRate = 8
        portalMarker.Image = p_game.LoadBitmap("Portal Marker.png")
        treeImage = p_game.LoadBitmap("trees64.png")
        MakeTileAnimations()
        LoadedRecently = False
    End Sub
    Public Function loadTilemap(ByVal filename As String) As Boolean
        Loadbg.Draw()
        StopAllBGMusic()
        clearCharactersAndItems()
        LoadedRecently = True
        Application.DoEvents()

        Try
            Dim doc As XmlDocument = New XmlDocument()
            doc.Load(filename)
            p_currentLevel = filename
            Dim nodelist As XmlNodeList = doc.GetElementsByTagName("tiles")
            For Each node As XmlNode In nodelist
                Dim element As XmlElement = node
                Dim index As Integer = 0
                Dim ts As New tilemapStruct
                Dim data As String

                REM read data fields from xml 
                data = element.GetElementsByTagName("tile")(0).InnerText
                index = Convert.ToInt32(data)
                data = element.GetElementsByTagName("value")(0).InnerText
                ts.tilenum = Convert.ToInt32(data)
                data = element.GetElementsByTagName("data1")(0).InnerText
                ts.data1 = Convert.ToString(data)
                data = element.GetElementsByTagName("data2")(0).InnerText
                ts.data2 = Convert.ToString(data)
                data = element.GetElementsByTagName("data3")(0).InnerText
                ts.data3 = Convert.ToString(data)
                data = element.GetElementsByTagName("data4")(0).InnerText
                ts.data4 = Convert.ToString(data)
                data = element.GetElementsByTagName("collidable")(0).InnerText
                ts.collidable = Convert.ToBoolean(data)
                data = element.GetElementsByTagName("portal")(0).InnerText
                ts.portal = Convert.ToBoolean(data)
                data = element.GetElementsByTagName("portalx")(0).InnerText
                ts.portalx = Convert.ToInt32(data)
                data = element.GetElementsByTagName("portaly")(0).InnerText
                ts.portaly = Convert.ToInt32(data)
                data = element.GetElementsByTagName("portalfile")(0).InnerText
                ts.portalfile = Convert.ToString(data)
                ts.tileX = Convert.ToInt32(element.GetElementsByTagName("tileY")(0).InnerText)
                ts.tileY = Convert.ToInt32(element.GetElementsByTagName("tileX")(0).InnerText)
                ts.tempCollidable = False

                REM store data in tilemap
                p_tilemap(index) = getTileAnim(ts)
            Next
        Catch es As Exception
            MessageBox.Show(es.Message)
            Return False
        End Try
        HandleCurrentLevel()
        HandleSpecialBgTilenums()
        p_game.p_frm.CheckCharacterSpawns()
        p_game.p_frm.CheckItemSpawns()
        p_game.quests.Visible = True
        p_game.Device.Clear(Color.Black)
        p_gfxScrollBuffer.Clear(Color.Black)
        fillScrollBuffer()
        Dim font As New Font("Narkisim", 24, FontStyle.Regular)
        Dim pos As PointF
        Dim sz As SizeF = p_game.Device.MeasureString(p_currentLevel.Substring(0, p_currentLevel.Length - 6), font)
        pos = New PointF(((Form1.Width / 2) - (sz.Width / 2)), 14)
        p_game.AddFadingText(pos, p_currentLevel.Substring(0, p_currentLevel.Length - 6), 4.0F, Color.White, font, False, New PointF(0, 0))
    End Function
    Public Function loadPalette(ByVal lightTiles As String, ByVal darkTiles As String, ByVal columns As Integer) As Boolean
        p_columns = columns
        Try
            p_bmpTiles = New Bitmap(lightTiles)
            p_bmpTilesDark = New Bitmap(darkTiles)
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function
#End Region
#Region "Methods"
    Public Sub Update()
        Dim steps As Integer = 4
        Static oldPlayerPos As PointF = p_game.hero.Position
        Static oldScrollPos As PointF = p_scrollPos
        oldPlayerPos = p_game.hero.Position
        oldScrollPos = p_scrollPos

        If p_game.hero.AnimationState = Character.AnimationStates.Attacking Or p_game.keyState.shift Then steps = 0

        REM only move the player if the player is not frozen

        If Not p_game.hero.Frozen Then
            REM up key movement
            If p_game.keyState.up Then
                If p_game.hero.Y > 300 - 48 Then
                    oldPlayerPos = p_game.hero.Position
                    p_game.hero.Y -= steps
                    If GetTile(p_game.hero.GetCurrentTilePos).collidable Or GetTile(p_game.hero.GetCurrentTilePos).tempCollidable Then
                        p_game.hero.Y += steps
                    End If
                Else
                    oldScrollPos = p_scrollPos
                    p_scrollPos.Y -= steps
                    If p_scrollPos.Y <= 0 Then
                        p_game.hero.Y -= steps
                    End If
                    If GetTile(p_game.hero.GetCurrentTilePos).collidable Or GetTile(p_game.hero.GetCurrentTilePos).tempCollidable Then
                        p_game.hero.Y += steps
                    End If
                End If

                REM down key movement
            ElseIf p_game.keyState.down Then
                If p_game.hero.Y < 300 - 48 Then
                    oldPlayerPos = p_game.hero.Position
                    p_game.hero.Y += steps
                    If GetTile(p_game.hero.GetCurrentTilePos).collidable Or GetTile(p_game.hero.GetCurrentTilePos).tempCollidable Then
                        p_game.hero.Y -= steps
                    End If
                Else
                    oldScrollPos = p_scrollPos
                    p_scrollPos.Y += steps
                    If p_scrollPos.Y >= (127 - 19) * 32 Then
                        p_game.hero.Y += steps
                    End If
                    If GetTile(p_game.hero.GetCurrentTilePos).collidable Or GetTile(p_game.hero.GetCurrentTilePos).tempCollidable Then
                        p_game.hero.Y -= steps
                    End If
                End If
            End If

            REM left key movement
            If p_game.keyState.left Then
                If p_game.hero.X > 400 - 48 Then
                    oldPlayerPos = p_game.hero.Position
                    p_game.hero.X -= steps
                Else
                    oldScrollPos = p_scrollPos
                    p_scrollPos.X -= steps
                    If p_scrollPos.X <= 0 Then
                        p_game.hero.X -= steps
                    End If
                    If GetTile(p_game.hero.GetCurrentTilePos).collidable Or GetTile(p_game.hero.GetCurrentTilePos).tempCollidable Then
                        p_game.hero.X += steps
                    End If
                End If

                REM right key movement
            ElseIf p_game.keyState.right Then
                If p_game.hero.X < 400 - 48 Then
                    oldPlayerPos = p_game.hero.Position
                    p_game.hero.X += steps
                Else
                    oldScrollPos = p_scrollPos
                    p_scrollPos.X += steps
                    If p_scrollPos.X >= (127 - 25) * 32 Then
                        p_game.hero.X += steps
                    End If
                    If GetTile(p_game.hero.GetCurrentTilePos).collidable Or GetTile(p_game.hero.GetCurrentTilePos).tempCollidable Then
                        p_game.hero.X -= steps
                    End If
                End If
            End If
        End If

        REM resolve collidable tile
        Dim pos As Point = p_game.hero.GetCurrentTilePos()
        p_currentTile = GetTile(pos.X, pos.Y)
        ' For n As Integer = (GridPos.X) To GridPos.X + CInt(Int(800 / 32)) Step 1
        '     For m As Integer = (GridPos.Y) To GridPos.Y + CInt(Int(600 / 32)) Step 1
        '         If GetTile(n, m).Bounds.IntersectsWith(p_game.hero.GetCurrentTileRect) Then
        '             If GetTile(n, m).collidable Or GetTile(n, m).tempCollidable Then
        '                 p_currentTile = GetTile(n, m)
        '             End If
        '         End If
        '     Next
        ' Next
        p_collidableFlag = p_currentTile.collidable
        If p_collidableFlag = False Then
            If GetTile(pos.X, pos.Y).tempCollidable Then
                p_collidableFlag = True
            End If
        End If
        If p_collidableFlag Then
            p_scrollPos = oldScrollPos
            p_game.hero.Position = oldPlayerPos
        End If

        REM resolve portal tile
        p_portalFlag = p_currentTile.portal
        If p_currentTile.portal Then
            If CurrentTile.portalfile <> "" Then
                loadTilemap(CurrentTile.portalfile)
            End If
            PlayTeleport()
            p_portalTarget = New Point(p_currentTile.portalx, p_currentTile.portaly)
            p_game.hero.SetFootPos(p_portalTarget.X, p_portalTarget.Y)
            p_scrollPos.X = p_game.hero.Position.X - (400 - 48)
            p_scrollPos.Y = p_game.hero.Position.Y - (300 - 48)
            If p_scrollPos.X < 0 Then p_scrollPos.X = 0
            If p_scrollPos.Y < 0 Then p_scrollPos.Y = 0
            If p_scrollPos.X > (128 * 32) - 800 Then p_scrollPos.X = (127 * 32) - 800
            If p_scrollPos.Y > (128 * 32) - 600 Then p_scrollPos.Y = (127 * 32) - 600
            p_game.hero.SetFootPos(p_portalTarget.X, p_portalTarget.Y)
            p_game.hero.X -= p_game.world.ScrollPos.X
            p_game.hero.Y -= p_game.world.ScrollPos.Y
            p_LastSpawn = p_game.hero.GetCurrentTilePos
        End If

        ValidateScrollPosition()

        REM fill the scroll buffer only when moving
        oldHour = My.Computer.Clock.LocalTime.Hour
        If p_scrollPos <> p_oldScrollPos Or My.Computer.Clock.LocalTime.Hour <> oldHour Then
            p_oldScrollPos = p_scrollPos

            ValidateScrollPosition()

            REM calculate sub-tile size
            p_subtile.X = p_scrollPos.X Mod p_tileSize
            p_subtile.Y = p_scrollPos.Y Mod p_tileSize

            REM fill scroll buffer with tiles
            fillScrollBuffer()
        End If

        AnimateTiles()
        For Each chest As Chest In chests
            chest.Update()
        Next
        For Each laserCannon As LaserCannon In laserCannons
            laserCannon.update()
        Next
        p_game.ChestFlag = False
        HandleDoorEvents()
        For Each Switch As Switch In switches
            Switch.Update()
        Next
        p_game.switchFlag = False


        PlayLevelMusic()
        If LoadedRecently Then LoadedRecently = False
    End Sub
    Private Sub StopAllBGMusic()
        StopDarkness()
        StopDrips()
    End Sub
    Private Sub PlayLevelMusic()
        Select Case p_currentLevel
            Case "Silent Temple F2.level", "Silent Temple F1.level"
                PlayDarkness()
        End Select
    End Sub
    Public Sub ValidateScrollPosition()
        REM validate X range
        If p_scrollPos.X < 0 Then p_scrollPos.X = 0
        If p_scrollPos.X > 3296 Then
            p_scrollPos.X = 3296
        End If

        REM validate Y range
        If p_scrollPos.Y < 0 Then p_scrollPos.Y = 0
        If p_scrollPos.Y > 3496 Then
            p_scrollPos.Y = 3496
        End If
    End Sub
    Public Sub drawTileNumber(ByVal x As Integer, ByVal y As Integer, ByVal tile As Integer)
        Dim sx As Integer = (tile Mod p_columns) * (p_tileSize + 1)
        Dim sy As Integer = (tile \ p_columns) * (p_tileSize + 1)
        Dim src As New Rectangle(sx, sy, p_tileSize, p_tileSize)
        Dim dx As Integer = x * p_tileSize
        Dim dy As Integer = y * p_tileSize
        p_gfxScrollBuffer.DrawImage(p_bmpTiles, dx, dy, src, GraphicsUnit.Pixel)
    End Sub
    Public Sub Draw(ByVal rect As Rectangle)
        Draw(rect.X, rect.Y, rect.Width, rect.Height)
    End Sub
    Public Sub Draw(ByVal width As Integer, ByVal height As Integer)
        Draw(0, 0, width, height)
    End Sub
    Public Sub Draw(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
        Update()
        Dim source As New Rectangle(p_subtile.X, p_subtile.Y, width, height)
            p_game.Device.DrawImage(p_bmpScrollBuffer, x, y, source, GraphicsUnit.Pixel)
        HandleStuctures()
        Select Case My.Computer.Clock.LocalTime.TimeOfDay.Hours
            Case 21, 22, 23, 24, 1, 2, 3, 4, 5
                Dim alpha As New Pen(Color.FromArgb(255 * 0.6, 0, 0, 0))
                p_game.Device.FillRectangle(alpha.Brush, 0, 0, p_game.p_frm.Width, p_game.p_frm.Height)
        End Select
        DrawPortalAnims()
    End Sub
    Private Sub fillScrollBuffer()
        Dim tilenum, sx, sy As Integer
        For tx = 0 To p_windowSize.X
            For ty = 0 To p_windowSize.Y
                sx = p_scrollPos.X \ p_tileSize + tx
                sy = p_scrollPos.Y \ p_tileSize + ty

                Dim tile As Integer = (sy * 128 + sx)

                If tile > p_tilemap.Count - 1 Then tile = p_tilemap.Count - 1
                tilenum = p_tilemap(tile).tilenum

                If p_tilemap(tile).bgTileNum <> -1 Then
                    If Not p_tilemap(tile).animate Then
                        p_game.Device.SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
                        drawTileNumberDay(tx, ty, p_tilemap(tile).bgTileNum)
                    End If

                    p_game.Device.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                End If


                drawTileNumberDay(tx, ty, tilenum)

                '  Dim currentTile As Point = New Point(p_scrollPos.X / p_tileSize, p_scrollPos.Y / p_tileSize)
                '  Dim playerTile As New Point(0, 0)
                '  playerTile = p_game.hero.GetCurrentTilePos()
                'If '((Math.Abs(sx - (playerTile.X + currentTile.X) - 1) <= 3) And
                '(Math.Abs(sy - (playerTile.Y + currentTile.Y) - 1) <= 3)) Then
                REM draw tile using light tileset
                'drawTileNumberDay(tx, ty, tilenum)
                'Else
                REM draw tile using dark tileset)
                '  drawTileNumberNight(tx, ty, tilenum)
            Next
        Next
    End Sub
    Public Sub drawTileNumberNight(ByVal x As Integer, ByVal y As Integer, ByVal tile As Integer)
        Dim currentTile As Point = New Point(p_scrollPos.X / p_tileSize, p_scrollPos.Y / p_tileSize)
        Dim sx As Integer = (tile Mod p_columns) * (p_tileSize + 1)
        Dim sy As Integer = (tile \ p_columns) * (p_tileSize + 1)
        Dim src As New Rectangle(sx, sy, p_tileSize, p_tileSize)
        Dim dx As Integer = x * p_tileSize
        Dim dy As Integer = y * p_tileSize
        p_gfxScrollBuffer.DrawImage(p_bmpTilesDark, dx, dy, src, GraphicsUnit.Pixel)
    End Sub
    Public Sub drawTileNumberDay(ByVal x As Integer, ByVal y As Integer, ByVal tile As Integer, Optional hasTransparancy As Boolean = False)
        Dim sx As Integer = (tile Mod p_columns) * (p_tileSize + 1)
        Dim sy As Integer = (tile \ p_columns) * (p_tileSize + 1)
        Dim src As New Rectangle(sx, sy, p_tileSize, p_tileSize)
        Dim dx As Integer = x * p_tileSize
        Dim dy As Integer = y * p_tileSize
        If Not hasTransparancy Then

        Else

        End If
        p_gfxScrollBuffer.DrawImage(p_bmpTiles, dx, dy, src, GraphicsUnit.Pixel)
    End Sub
    Public Sub HandleStuctures()
        Dim heroPos As Point = p_game.hero.GetCurrentTilePos()
        For Each stucture As DrawableStructure In DrawableStructures
            If IsOnScreen(New Rectangle(stucture.relativePos.X, stucture.relativePos.Y, stucture.sprite.Width, stucture.sprite.Width)) Then
                stucture.relativePos.X = stucture.relativePos.X - p_game.world.ScrollPos.X
                stucture.relativePos.Y = stucture.relativePos.Y - p_game.world.ScrollPos.Y
                stucture.sprite.Draw(stucture.relativePos.X, stucture.relativePos.Y)
            End If
            Select Case My.Computer.Clock.LocalTime.TimeOfDay.Hours
                Case 21, 22, 23, 24, 1, 2, 3, 4, 5
                    Dim alpha As New Pen(Color.FromArgb(255 * 0.6, 0, 0, 0))
                    Dim rx As Integer = stucture.relativePos.X
                    Dim ry As Integer = stucture.relativePos.Y
                    Dim sx As Integer = stucture.bmp.Width
                    Dim sy As Integer = stucture.bmp.Height
                    'p_game.Device.FillRectangle(alpha.Brush, rx, ry, sx, sy)
            End Select
        Next
        Dim stopFire As Boolean = True
        For Each fire As DrawableScript In animationScripts
            If IsOnScreen(New Rectangle(fire.relativePos.X, fire.relativePos.Y, fire.sprite.Width, fire.sprite.Width)) Then
                fire.relativePos.X = fire.relativePos.X - p_game.world.ScrollPos.X
                fire.relativePos.Y = fire.relativePos.Y - p_game.world.ScrollPos.Y
                fire.sprite.Animate(0, fire.sprite.Columns - 1)
                fire.sprite.Draw(fire.relativePos.X, fire.relativePos.Y)
                If GetTile(heroPos.X, heroPos.Y).data1 = "fire" Or GetTile(heroPos.X, heroPos.Y).data1 = "Lethal" Or GetTile(heroPos.X, heroPos.Y).data1 = "lethal" Then
                    If p_game.hero.Alive Then p_game.hero.Health -= 0.083333333333333329
                    p_game.combat.CheckPlayerDeath()
                End If
                For Each monster As Character In p_game.NPCs
                    If GetTile(monster.GetCurrentTilePos.X, monster.GetCurrentTilePos.Y).data1 = "fire" Or GetTile(monster.X, monster.Y).data1 = "Lethal" Or GetTile(monster.X, monster.Y).data1 = "lethal" Then
                        If monster.Alive Then monster.Health -= 0.083333333333333329
                        p_game.combat.Target = monster
                        p_game.combat.CheckEnemyDeath()
                    End If
                Next
                For Each animal As Animal In p_game.animals
                    If GetTile(animal.GetCurrentTilePos.X, animal.GetCurrentTilePos.Y).data1 = "fire" Or GetTile(animal.X, animal.Y).data1 = "Lethal" Or GetTile(animal.X, animal.Y).data1 = "lethal" Then
                        If animal.Alive Then animal.Health -= 0.083333333333333329
                        If animal.Health <= 0 Then
                            animal.Alive = False
                            animal.AnimationState = Character.AnimationStates.Dying
                        End If
                    End If
                Next
                PlayFire()
                stopFire = False
            Else
                fire.relativePos.X += p_game.world.ScrollPos.X
                fire.relativePos.Y += p_game.world.ScrollPos.Y
            End If
        Next
        For Each chest As Chest In chests
            chest.DrawToScreen()
        Next
        For Each door As Door In doors
            door.Update()
            door.DrawToScreen()
        Next
        p_game.doorFlag = False
        If stopFire Then SoundManager.StopFire()
        DrawTrees()
        For Each laserCannon As LaserCannon In laserCannons
            laserCannon.Draw()
        Next
        For Each Switch As Switch In switches
            Switch.DrawToScreen()
        Next
    End Sub
    Public Sub HandleCurrentLevel()
        For n As Integer = 0 To p_tilemap.Count - 1
            If p_tilemap(n).data1 = "item" Or p_tilemap(n).data1 = "item" Then
                p_game.LuaDropItem(p_tilemap(n).data2, CInt(p_tilemap(n).data3), CInt(p_tilemap(n).data4))
            ElseIf p_tilemap(n).data1 = "gold" Then
                If p_tilemap(n).data3 = "" AndAlso p_tilemap(n).data4 = "" Then
                    p_game.LuaDropGold(CInt(p_tilemap(n).data2), p_tilemap(n).tileX * p_tileSize, p_tilemap(n).tileY * p_tileSize)
                Else
                    p_game.LuaDropGold(CInt(p_tilemap(n).data2), CInt(p_tilemap(n).data3), CInt(p_tilemap(n).data4))
                End If
            ElseIf p_tilemap(n).data1 = "NPC" Then
                Dim character As Character = p_game.LuaMakeCharacter(p_tilemap(n).data2, p_tilemap(n).data3, p_tilemap(n).data4, 150, 150)
                Select Case character.Name
                    Case "Tutorial Knight1"
                        MakeTutorialKnight1()
                    Case "Tutorial Knight2"
                        MakeTutorialKnight2()
                End Select
            ElseIf p_tilemap(n).data1 = "character" Or p_tilemap(n).data1 = "Character" Then
                If p_tilemap(n).data3 = "" Or p_tilemap(n).data4 = "" Then
                    Dim character As Character = p_game.LuaMakeCharacter(p_tilemap(n).data2, p_tilemap(n).tileX * 32, p_tilemap(n).tileY * 32, 150, 150)
                    Dim spawnPoint As Point = New Point(p_tilemap(n).tileX, p_tilemap(n).tileY)
                    character.SetFootPos(spawnPoint.X, spawnPoint.Y)
                    p_game.LuaAddCharacter(character)
                Else
                    Dim character As Character = p_game.LuaMakeCharacter(p_tilemap(n).data2, p_tilemap(n).data3, p_tilemap(n).data4, 150, 150)
                    Dim spawnPoint As Point = New Point(CInt(p_tilemap(n).data3) / 32, CInt(p_tilemap(n).data4) / 32)
                    character.SetFootPos(spawnPoint.X, spawnPoint.Y)
                    p_game.LuaAddCharacter(character)
                End If
            ElseIf p_tilemap(n).data1 = "Event Monster" Or p_tilemap(n).data1 = "event monster" Then
                If p_tilemap(n).data3 = "" Or p_tilemap(n).data4 = "" Then
                    Dim character As Character = p_game.LuaMakeCharacter(p_tilemap(n).data2, p_tilemap(n).tileX * 32, p_tilemap(n).tileY * 32, 150, 150)
                    Dim spawnPoint As Point = New Point(p_tilemap(n).tileX, p_tilemap(n).tileY)
                    character.SetFootPos(spawnPoint.X, spawnPoint.Y)
                    p_game.eventMonsters.Add(character)
                Else
                    Dim character As Character = p_game.LuaMakeCharacter(p_tilemap(n).data2, p_tilemap(n).data3, p_tilemap(n).data4, 150, 150)
                    Dim spawnPoint As Point = New Point(CInt(p_tilemap(n).data3) / 32, CInt(p_tilemap(n).data4) / 32)
                    character.SetFootPos(spawnPoint.X, spawnPoint.Y)
                    p_game.eventMonsters.Add(character)
                End If
            ElseIf p_tilemap(n).data1 = "Structure" Then
                Dim struct As DrawableStructure
                struct.bmp = p_game.LoadBitmap(p_tilemap(n).data2)
                struct.sprite = New Sprite(p_game)
                struct.sprite.Image = struct.bmp
                struct.sprite.Size = New Size(struct.bmp.Width, struct.bmp.Height)
                struct.relativePos = New Point(CInt(p_tilemap(n).data3), CInt(p_tilemap(n).data4))
                DrawableStructures.Add(struct)
            ElseIf p_tilemap(n).data1 = "fire" Or p_tilemap(n).data1 = "Fire" Then
                Dim struct As New DrawableScript
                struct.bmp = p_game.LoadBitmap("fire.png")
                struct.sprite = New Sprite(p_game)
                struct.sprite.Image = struct.bmp
                struct.sprite.Size = New Size(32, 32)
                struct.sprite.Columns = 20
                struct.sprite.CurrentFrame = 0
                struct.sprite.Alive = True
                struct.scriptName = "fire"
                If p_tilemap(n).data3 = "" And p_tilemap(n).data4 = "" Then
                    struct.relativePos = New Point(p_tilemap(n).tileX * p_tileSize, p_tilemap(n).tileY * p_tileSize)
                Else
                    struct.relativePos = New Point(CInt(Val(p_tilemap(n).data3)) * 32, (CInt(Val(p_tilemap(n).data4)) * 32) - (struct.sprite.Size.Width / 2))
                End If
                animationScripts.Add(struct)
            ElseIf p_tilemap(n).data1 = "drawablescript" Then
                Dim struct As New DrawableScript
                struct.bmp = p_game.LoadBitmap(p_tilemap(n).data2)
                struct.sprite = New Sprite(p_game)
                struct.sprite.Image = struct.bmp
                struct.sprite.Size = New Size(32, 32)
                struct.sprite.Columns = 20
                struct.sprite.CurrentFrame = 0
                struct.sprite.Alive = True
                struct.scriptName = "drawablescript"
                If p_tilemap(n).data3 = "" And p_tilemap(n).data4 = "" Then
                    struct.relativePos = New Point(p_tilemap(n).tileX * p_tileSize, p_tilemap(n).tileY * p_tileSize)
                Else
                    struct.relativePos = New Point(CInt(Val(p_tilemap(n).data3)) * 32, (CInt(Val(p_tilemap(n).data4)) * 32) - (struct.sprite.Size.Width / 2))
                End If
                animationScripts.Add(struct)
            ElseIf p_tilemap(n).data1 = "Tree" Or p_tilemap(n).data1 = "tree" Then
                Dim tree As New Tree
                If (p_tilemap(n).data3 = "") AndAlso (p_tilemap(n).data4 = "") Then
                    tree.relativePos.X = p_tilemap(n).tileX * p_tileSize
                    tree.relativePos.Y = p_tilemap(n).tileY * p_tileSize
                Else
                    tree.relativePos.X = p_tileSize * Val(p_tilemap(n).data3)
                    tree.relativePos.Y = p_tileSize * Val(p_tilemap(n).data4)
                End If
                tree.srcRect.X = (Val(p_tilemap(n).data2) Mod 4) * tree.width
                tree.srcRect.Y = (Val(p_tilemap(n).data2) \ 4) * tree.height
                tree.srcRect.Width = tree.width
                tree.srcRect.Height = tree.height
                For v As Integer = p_tilemap(n).tileX To p_tilemap(n).tileX + 4
                    For m As Integer = p_tilemap(n).tileY To p_tilemap(n).tileY + 4
                        SetCollidable(New PointF(v, m), True)
                    Next
                Next
                trees.Add(tree)
            ElseIf p_tilemap(n).data1 = "Chest" Or p_tilemap(n).data1 = "chest" Then
                Dim value As Integer
                If p_tilemap(n).data4 = "" Then
                    value = 0
                Else
                    value = CInt(p_tilemap(n).data4)
                End If
                AddChest(CBool(p_tilemap(n).data2), p_tilemap(n).data3, value, p_tilemap(n).tileX * p_tileSize, p_tilemap(n).tileY * p_tileSize)
                p_tilemap(n).collidable = True
            ElseIf p_tilemap(n).data1 = "Door" Or p_tilemap(n).data1 = "door" Then
                Try
                    AddDoor(CInt(p_tilemap(n).data2), p_tilemap(n).data3, p_tilemap(n).tileX * p_tileSize, p_tilemap(n).tileY * p_tileSize)
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            ElseIf p_tilemap(n).data1 = "laser cannon" Or p_tilemap(n).data1 = "Laser Cannon" Then
                Dim laserCannon As New LaserCannon(p_game, CInt(p_tilemap(n).data2), p_tilemap(n).tileX, p_tilemap(n).tileY)
                laserCannons.Add(laserCannon)
            ElseIf p_tilemap(n).data1 = "animal" Or p_tilemap(n).data1 = "Animal" Then
                AddAnimal(p_tilemap(n).data2, p_tilemap(n).tileX, p_tilemap(n).tileY)
            ElseIf p_tilemap(n).data1 = "Switch" Or p_tilemap(n).data1 = "switch" Then
                switches.Add(New Switch(p_game, CInt(p_tilemap(n).data2), p_tilemap(n).tileX * 32, p_tilemap(n).tileY * 32))
            End If

            'configure transparency options of tile
            If n < p_tilemap.Count - 1 Then
                Dim num As Integer = p_tilemap(n).tilenum
                Select Case p_tilemap(n).tilenum
                    Case 207, 208, 247, 285, 287, 255, 257, 315, 317, 179, 189, 188, 284, 206, 205, 318, 328, 590, 591, 592, 593, 594, 595, 599, _
                        600, 601, 602, 603, 604, 605, 608, 609, 610, 611, 612, 613, 614, 615, 616, 617, 618, 619, 620, 621, 622, 623, 624, 625, 626, _
                        629, 630, 631, 632, 633, 634, 635, 636, 640, 641
                        num = Val(p_tilemap(n).data1)
                    Case Else
                        num = -1
                End Select
                p_tilemap(n).bgTileNum = num
            End If

        Next
    End Sub
    Public Sub clearCharactersAndItems()
        p_game.NPCs.Clear()
        If p_game.tutorialKnight1 IsNot Nothing Then
            p_game.tutorialKnight1.Visible = False
            p_game.tutorialKnight1 = Nothing
        End If
        If p_game.tutorialKnight2 IsNot Nothing Then
            p_game.tutorialKnight2.Visible = False
            p_game.tutorialKnight2 = Nothing
        End If
        If p_game.Vendor IsNot Nothing Then
            p_game.Vendor.Visible = False
            p_game.Vendor = Nothing
        End If
        If p_game.Urix IsNot Nothing Then
            p_game.Urix.Visible = False
            p_game.Urix = Nothing
        End If
        p_game.treasure.Clear()
        DrawableStructures.Clear()
        animationScripts.Clear()
        p_game.heroProjectiles.Clear()
        p_game.monsterProjectiles.Clear()
        chests.Clear()
        trees.Clear()
        doors.Clear()
        laserCannons.Clear()
        p_game.animals.Clear()
        p_game.eventMonsters.Clear()
        switches.Clear()
        p_game.fadingTexts.Clear()
    End Sub
    Public Sub DrawTrees()
        For Each Tree As Tree In trees
            If IsOnScreen(Tree.Bounds) Then
                Try
                    p_game.Device.DrawImage(treeImage, New Rectangle(Tree.Bounds.X - ScrollPos.X, Tree.Bounds.Y - ScrollPos.Y, Tree.width, Tree.height), Tree.srcRect, GraphicsUnit.Pixel)
                Catch ex As Exception
                    Return
                End Try
            End If
        Next
    End Sub
    '  Public Sub currentLevelLogic()
    '         Select p_currentLevel
    '            Case "Kildare.level"
    '               Select Case p_game.quests.QuestNumber
    '                  Case 0
    '                     p_game.LuaDropGold(2, 900, 420)
    '                    p_game.LuaDropGold(4, 940, 440)
    '                   p_game.LuaDropGold(3, 920, 430)
    '                  p_game.LuaDropGold(1, 910, 420)
    '                 p_game.LuaDropGold(5, 930, 420)
    '            Case 3
    '               If Not p_game.inven.HasItem("Red Key") Then
    '                  p_game.LuaDropItem("Red Key", 700, 450)
    '             End If
    '      End Select
    '  End Select
    'End Sub
    Public Sub DrawProperly(bmp As Bitmap, x As Single, y As Single)
        If ScrollPos.X <= 0 Or ScrollPos.Y <= 0 Or ScrollPos.Y >= 3988 Or ScrollPos.X >= 3988 Then
            p_gfxScrollBuffer.DrawImage(bmp, New Point(x, y))
        Else
            p_game.Device.DrawImage(bmp, New Point(x, y))
        End If
    End Sub
    Public Sub DrawProperly(bmp As Bitmap, srcRect As Rectangle, dest As Rectangle)
        If ScrollPos.X <= 0 Or ScrollPos.Y <= 0 Or ScrollPos.Y >= 3988 Or ScrollPos.X >= 3988 Then
            p_gfxScrollBuffer.DrawImage(bmp, dest, srcRect, GraphicsUnit.Pixel)
        Else
            p_game.Device.DrawImage(bmp, dest, srcRect, GraphicsUnit.Pixel)
        End If
    End Sub
    Private Sub HandleSpecialBgTilenums()
        Select Case p_currentLevel
            Case "Silent Temple F1.level"
                Dim sucess As Boolean = False
                For m As Integer = 56 To 60
                    Dim ts As tilemapStruct = GetTile(m, 7)
                    ts.bgTileNum = 407
                    p_tilemap(7 * 128 + m) = ts
                Next
                For m As Integer = 4 To 7
                    Dim ts As tilemapStruct = GetTile(56, m)
                    ts.bgTileNum = 407
                    p_tilemap(m * 128 + 56) = ts
                Next
                For m As Integer = 56 To 60
                    Dim ts As tilemapStruct = GetTile(m, 6)
                    ts.bgTileNum = 407
                    p_tilemap(6 * 128 + m) = ts
                Next
                For m As Integer = 4 To 7
                    Dim ts As tilemapStruct = GetTile(60, m)
                    ts.bgTileNum = 407
                    p_tilemap(m * 128 + 60) = ts
                Next
        End Select
    End Sub
    Public Sub HandleDoorEvents()
        If p_game.gameState <> Game.GameStates.STATE_PLAYING Then Return
        Static oldSwitchList As New List(Of Integer)
        Static doOnce As Boolean = False
        Static doOnce1 As Boolean = False
        Static doOnce2 As Boolean = False
        Static doOnce3 As Boolean = False
        Static doOnce4 As Boolean = False
        Static doOnce5 As Boolean = False
        Static doOnce6 As Boolean = False
        Static doOnce7 As Boolean = False
        If LoadedRecently Then
            doOnce = False
            doOnce1 = False
            doOnce2 = False
            doOnce3 = False
            doOnce4 = False
            doOnce5 = False
            doOnce6 = False
            doOnce7 = False
            oldSwitchList.Clear()
        End If
        Select Case p_currentLevel
            Case "Silent Forest(Inner).level"
                If p_game.quests.QuestNumber >= 11 Then
                    For Each d As Door In doors
                        d.ToggleLocked(False)
                        d.OpenDoor()
                    Next
                End If
            Case "Silent Temple F1.level"
                If (p_game.eventMonsters(0).Alive = False AndAlso p_game.eventMonsters(1).Alive = False AndAlso p_game.eventMonsters(2).Alive = False _
                AndAlso p_game.eventMonsters(3).Alive = False) Then
                    doors(32).ToggleLocked(False)
                    doors(35).ToggleLocked(False)
                    'ElseIf (p_game.hero.GetCurrentTilePos.X < 45 And p_game.hero.GetCurrentTilePos.Y < 2) Then
                    '   p_game.eventMonsters(0).KillInstantly()
                    '  p_game.eventMonsters(1).KillInstantly()
                    '  p_game.eventMonsters(2).KillInstantly()
                    '  p_game.eventMonsters(3).KillInstantly()
                    '  doors(32).ToggleLocked(False)
                    '   doors(35).ToggleLocked(False)
                ElseIf ((p_game.hero.GetCurrentTilePos.X < 42 AndAlso p_game.hero.GetCurrentTilePos.Y > 110)) AndAlso ((p_game.eventMonsters(0).Alive = True Or p_game.eventMonsters(1).Alive = True Or p_game.eventMonsters(2).Alive = True _
                Or p_game.eventMonsters(3).Alive = True)) Then
                    doors(35).ToggleLocked(True)
                ElseIf Not ((p_game.hero.GetCurrentTilePos.X < 42 AndAlso p_game.hero.GetCurrentTilePos.Y > 110)) AndAlso ((p_game.eventMonsters(0).Alive = True Or p_game.eventMonsters(1).Alive = True Or p_game.eventMonsters(2).Alive = True _
                Or p_game.eventMonsters(3).Alive = True)) Then
                    doors(35).ToggleLocked(False)
                End If

                If doors(33).IsLocked = False Or doors(34).IsLocked = False Then
                    doors(34).ToggleLocked(False)
                    doors(33).ToggleLocked(False)
                End If

                If switches(0).IsFlipped Then
                    For n As Integer = 14 To 31 Step 1
                        doors(n).ToggleLocked(False)
                    Next
                End If

                For n As Integer = 0 To 13 Step 1
                    If doors(n).IsLocked = False Then
                        For m As Integer = 0 To 13 Step 1
                            doors(m).ToggleLocked(False)
                        Next
                        For m As Integer = 56 To 60
                            Dim ts As tilemapStruct = GetTile(m, 7)
                            ts.tilenum = 407
                            p_tilemap(7 * 128 + m) = ts
                        Next
                        For m As Integer = 4 To 7
                            Dim ts As tilemapStruct = GetTile(56, m)
                            ts.tilenum = 407
                            p_tilemap(m * 128 + 56) = ts
                        Next
                        For m As Integer = 56 To 60
                            Dim ts As tilemapStruct = GetTile(m, 6)
                            ts.tilenum = 407
                            p_tilemap(6 * 128 + m) = ts
                        Next
                        For m As Integer = 4 To 7
                            Dim ts As tilemapStruct = GetTile(60, m)
                            ts.tilenum = 407
                            p_tilemap(m * 128 + 60) = ts
                        Next
                        If doOnce = False Then
                            fillScrollBuffer()
                            doOnce = True
                        End If
                    End If
                Next
            Case "Silent Temple F2.level"
                If switches(0).IsFlipped Then
                    If doOnce = False Then
                        doOnce = True
                        oldSwitchList.Add(0)
                    End If
                End If
                If switches(1).IsFlipped Then
                    If doOnce1 = False Then
                        doOnce1 = True
                        oldSwitchList.Add(1)
                    End If
                End If
                If switches(2).IsFlipped Then
                    If doOnce2 = False Then
                        doOnce2 = True
                        oldSwitchList.Add(2)
                    End If
                End If

                If oldSwitchList.Count = 3 Then
                    If (switches(0).IsFlipped = True And switches(1).IsFlipped = True And switches(2).IsFlipped = True) AndAlso _
                                     ((oldSwitchList(0) = 0) And (oldSwitchList(1) = 2) And (oldSwitchList(2) = 1)) Then
                        For m As Integer = 63 To 64 Step 1
                            For v As Integer = 7 To 8 Step 1
                                Dim ts As tilemapStruct = GetTile(m, v)
                                ts.portal = True
                                ts.tilenum = 590
                                p_tilemap(v * 128 + m) = ts
                            Next
                        Next
                        If doOnce3 = False Then
                            fillScrollBuffer()
                            doOnce3 = False
                        End If
                        oldSwitchList.Clear()
                    Else
                        For n As Integer = 0 To 3
                            Dim c As New Character(p_game)
                            c.Load("Green Spider.char")
                            If n = 0 Then
                                c.SetFootPos(55, 7)
                            ElseIf n = 1 Then
                                c.SetFootPos(69, 7)
                            ElseIf n = 2 Then
                                c.SetFootPos(63, 13)
                            End If
                            c.range = New Rectangle(c.X, c.Y, 100, 100)
                            p_game.LuaAddCharacter(c)
                        Next
                        oldSwitchList.Clear()
                        doOnce1 = False
                        doOnce = False
                        doOnce2 = False
                        For Each Switch As Switch In switches
                            Switch.ToggleFilipped(False)
                        Next
                    End If
                End If

                'rem handle boss trap

                If doOnce5 = False Then
                    doOnce5 = True
                    For Each Door As Door In doors
                        Door.ToggleLocked(False)
                    Next
                End If

                For n As Integer = 54 To 75 Step 1
                    For m As Integer = 38 To 51
                        If doOnce4 = False And New Point(p_game.hero.GetCurrentTilePos.X, p_game.hero.GetCurrentTilePos.Y) = _
                        New Point(n, m) Then
                            doOnce4 = True
                            For Each Door As Door In doors
                                Door.ToggleLocked(True)
                            Next
                            Dim c As Character = New Character(p_game)
                            c.Load("Swamp Monster.char")
                            c.Position = New PointF(2049 - 24, 1442 - 24)
                            c.range = New Rectangle(54 * 32, 39 * 32, 76 * 32, 52 * 32)
                            p_game.eventMonsters.Add(c)
                            Exit For
                        End If
                    Next
                Next

                REM handle boss death
                If doOnce7 = False AndAlso p_game.eventMonsters.Count > 0 AndAlso p_game.eventMonsters(0).Alive = False AndAlso _
                    p_game.eventMonsters(0).AnimationState = Character.AnimationStates.Dead Then
                    doOnce7 = True
                    For Each Door As Door In doors
                        Door.ToggleLocked(False)
                    Next
                    Dim ts As tilemapStruct = p_tilemap(34 * 128 + 64)
                    ts.portal = True
                    ts.tilenum = 590
                    ts.bgTileNum = 614
                    p_tilemap(34 * 128 + 64) = ts
                    fillScrollBuffer()
                    p_game.LuaDropItem("Green Key", 2049, 1442)
                End If

        End Select

        ' For Each door As Door In doors
        '     p_game.AddFadingText(door.CurrentTilePos.X * 32, door.CurrentTilePos.Y * 32, doors.IndexOf(door).ToString, 0.05F, Brushes.White, New Font("Arial", 12))
        ' Next
    End Sub
#Region "Character Create Methods"
    Public Sub MakeUrix()
        p_game.Urix = New Character(p_game)
        p_game.Urix.Load("Urix.char")
        p_game.Urix.Position = New Point(2080, 2620 - 200)
        p_game.Urix.range = New Rectangle(2080, 2620 - 200, 150, 150)
        p_game.Urix.Alive = True
        p_game.Urix.AnimationState = Character.AnimationStates.Walking
        p_game.Urix.Direction = 4
    End Sub
    Public Sub MakePeasant()
        'p_game.Peasant = New Wounded_Farmer(p_game)
        p_game.Peasant = New Character(p_game)
        p_game.Peasant.Load("Wounded Farmer.char")
        p_game.Peasant.Position = New Point(1280, 288)
        p_game.Peasant.range = New Rectangle(1280, 288, 150, 150)
        p_game.Peasant.Alive = True
        p_game.Peasant.AnimationState = Character.AnimationStates.Walking
        p_game.Peasant.Direction = 4
    End Sub
    Public Sub MakeBlueKnight()
        p_game.BlueKnight = New Character(p_game)
        p_game.BlueKnight.Load("Blue Knight.char")
        p_game.BlueKnight.Position = New Point(3000, 900)
        p_game.BlueKnight.range = New Rectangle(96, 96, 96, 96)
        p_game.BlueKnight.AnimationState = Character.AnimationStates.Standing
        p_game.BlueKnight.Direction = 4
    End Sub
    Public Sub MakeTutorialKnight1()
        p_game.tutorialKnight1 = New Character(p_game)
        p_game.tutorialKnight1.Load("Full Plated Knight.char")
        p_game.tutorialKnight1.Position = New Point(880, 400)
        p_game.tutorialKnight1.Alive = True
        p_game.tutorialKnight1.range = New Rectangle(10, 10, 10, 10)
        p_game.tutorialKnight1.AnimationState = Character.AnimationStates.Standing
        p_game.tutorialKnight1.Direction = 4
    End Sub
    Public Sub MakeTutorialKnight2()
        p_game.tutorialKnight2 = New Character(p_game)
        p_game.tutorialKnight2.Load("Full Plated Knight.char")
        p_game.tutorialKnight2.Position = New PointF(624, 196)
        p_game.tutorialKnight2.range = New Rectangle(10, 10, 10, 10)
        p_game.tutorialKnight2.AnimationState = Character.AnimationStates.Standing
        p_game.tutorialKnight2.Direction = 4
    End Sub
    Public Sub MakeVendor()
        p_game.Vendor = New Character(p_game)
        p_game.Vendor.Load("Vendor.char")
        'p_game.Vendor.SetFootPos(272, 2480)
        p_game.Vendor.Position = New PointF(272, 2480)
        p_game.Vendor.range = New Rectangle(10, 10, 10, 10)
        p_game.Vendor.AnimationState = Character.AnimationStates.Standing
        p_game.Vendor.Direction = 4
    End Sub
#End Region
    Public Function IsOnScreen(rect As Rectangle) As Boolean
        If rect.IntersectsWith(New Rectangle(ScrollPos.X, ScrollPos.Y, p_windowSize.X * p_tileSize, p_windowSize.Y * p_tileSize)) Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function getTilePos(num As Integer) As Point
        Dim sx As Integer = (num Mod p_columns) * (p_tileSize + 1)
        Dim sy As Integer = (num \ p_columns) * (p_tileSize + 1)
        Dim src As New Point(sx, sy)
        Return src
    End Function
    Public Function ToWorld(pos As PointF) As PointF
        pos.X += ScrollPos.X
        pos.Y += ScrollPos.Y
        Return pos
    End Function
    Public Function ToWorld(pos As Point) As Point
        pos.X += ScrollPos.X
        pos.Y += ScrollPos.Y
        Return pos
    End Function
    Public Function ToScreen(pos As PointF) As PointF
        pos.X -= ScrollPos.X
        pos.Y -= ScrollPos.Y
        Return pos
    End Function
    Public Function ToScreen(pos As Point) As Point
        pos.X -= ScrollPos.X
        pos.Y -= ScrollPos.Y
        Return pos
    End Function
    Public Sub DrawPortalAnims()
        portalMarker.Animate()
        For n As Integer = GridPos.X To GridPos.X + (800 / 32) Step 1
            For m As Integer = GridPos.Y To GridPos.Y + (600 / 32) Step 1
                If GetTile(n, m).portal Then
                    portalMarker.Draw((n * 32) - ScrollPos.X, (m * 32) - ScrollPos.Y)
                End If
            Next
        Next
    End Sub
#Region "Tile Animation Fuctions"
    Public Sub MakeTileAnimations()
        'lava animation
        Dim lavaSprite As New Sprite(p_game)
        lavaSprite.Image = p_game.LoadBitmap("lava_Anim.png")
        tileAnims.Add(lavaSprite)
        'water animation
        Dim waterSprite As New Sprite(p_game)
        waterSprite.Image = p_game.LoadBitmap("water_Anim.png")
        tileAnims.Add(waterSprite)
        'water flowing animation
        Dim waterFlowingSprite As New Sprite(p_game)
        waterFlowingSprite.Image = p_game.LoadBitmap("water_flowing_Anim.png")
        tileAnims.Add(waterFlowingSprite)
        'poison anim
        Dim poisonSprite As New Sprite(p_game)
        poisonSprite.Image = p_game.LoadBitmap("poison_Anim.png")
        tileAnims.Add(poisonSprite)
        'poison tree anim
        Dim poisonTreeSprite As New Sprite(p_game)
        poisonSprite.Image = p_game.LoadBitmap("poison_tree_Anim.png")
        tileAnims.Add(poisonSprite)

        'set common tile sprite properties
        For Each Sprite As Sprite In tileAnims
            Sprite.Size = New Size(32, 32)
            Sprite.Columns = 3
            Sprite.Alive = True
            Sprite.AnimationRate = 5
            Sprite.CurrentFrame = 0
            Sprite.TotalFrames = 3
        Next
    End Sub
    Public Function getTileAnim(tile As tilemapStruct) As tilemapStruct
        Select Case tile.tilenum
            Case 219
                tile.animate = True
                tile.animationNum = 0
            Case 249
                tile.animate = True
                tile.animationNum = 1
            Case 229
                tile.animate = True
                tile.animationNum = 2
            Case 313
                tile.animate = True
                tile.animationNum = 3
            Case 239
                tile.animate = True
                tile.animationNum = 4
            Case Else
                tile.animate = False
        End Select
        Return tile
    End Function
    Public Sub DrawTileAnim(x As Integer, y As Integer, tileNum As Integer, tile As Integer)
        Dim ts As tilemapStruct = GetTile(tile)
        If New Rectangle(ScrollPos.X, ScrollPos.Y, 800, 600).IntersectsWith(ts.Bounds) Then
            tileAnims(p_tilemap(tile).animationNum).Draw((x * p_tileSize), (y * p_tileSize), p_gfxScrollBuffer)

            'play tile animation sounds
            If CurrentLevel = "default.level" Then Return
            If ts.tilenum = 219 Then
                PlayLava()
            ElseIf ts.tilenum = 249 Then
                PlayWater()
            End If
            If Not ts.tilenum = 249 Then StopWater()
            If Not ts.tilenum = 219 Then StopLava()
        End If
    End Sub
    Public Sub AnimateTiles()
        For Each anim As Sprite In tileAnims
            anim.Animate()
        Next

        Dim tilenum, sx, sy As Integer
        For tx = 0 To p_windowSize.X
            For ty = 0 To p_windowSize.Y
                sx = p_scrollPos.X \ p_tileSize + tx
                sy = p_scrollPos.Y \ p_tileSize + ty
                Dim tile As Integer = (sy * 128 + sx)
                If tile > p_tilemap.Count - 1 Then tile = p_tilemap.Count - 1
                If tile < 0 Then tile = 0
                If p_tilemap(tile).animate Then DrawTileAnim(tx, ty, tilenum, tile)
            Next
        Next
    End Sub
#End Region
#End Region
End Class