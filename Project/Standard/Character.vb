Imports System.Xml
Public Class Character
    Public Enum AnimationStates
        Standing
        Walking
        Attacking
        Talking
        Dying
        Dead
    End Enum
    Private p_game As Game
    Protected p_position As PointF
    Protected p_direction As Integer
    Protected p_state As AnimationStates
    Public range As Rectangle
    Private p_moving As Boolean
    Private Const attackDelay As Integer = 33
    Private attackTime As Integer = 0
    Private p_name As String
    Private p_class As String
    Private p_race As String
    Private p_desc As String
    Private p_str As Integer
    Private p_dex As Integer
    Private p_sta As Integer
    Private p_int As Integer
    Private p_cha As Integer
    Private p_hitpoints As Integer
    Private p_health As Single
    Private p_experience As Integer
    Private p_level As Integer
    Private p_alive As Boolean
    Private p_dropGold1 As Integer
    Private p_dropGold2 As Integer
    Private p_walkFilename As String
    Protected p_walkSprite As Sprite
    Private p_walkSize As Point
    Public p_walkColumns As Integer
    Private p_attackFilename As String
    Protected p_attackSprite As Sprite
    Private p_attackSize As Point
    Protected p_attackColumns As Integer
    Private p_dieFilename As String
    Protected p_dieSprite As Sprite
    Private p_dieSize As Point
    Protected p_dieColumns As Integer
    Private talkSize As Point
    Private talkFilname As String
    Protected talkSprite As Sprite
    Protected talkColumns As Integer
    Private p_dropnum1 As Integer
    Private p_dropnum2 As Integer
    Private p_dropnum3 As Integer
    Private p_dropitem1 As String
    Private p_dropitem2 As String
    Private p_dropitem3 As String
    Private p_visible As Boolean
    Private p_AttackFlag As Boolean
    Private _bleeding As Boolean
    Private _regen As Boolean
    Public _bleedTime As Single = 0.0F
    Public _regenTime As Single = 0.0F
    Private _freeze As Boolean
    Public _freezeTime As Single = 0.0F
    Public bleedPower As Single = 0.0F
    Public regenPower As Single = 0.0F
    Private _element As String = ""
    Public fireSprite As Sprite
    Public iceSprite As Sprite
    Public drainVictim As Character
    Public platformSprite As Sprite
    Protected oldTilePos As Point = New Point(-1, -1)
    Protected newTilePos As Point = New Point(-1, -1)
    Private oldState As AnimationStates
    Private newState As AnimationStates
    Private p_chase As Boolean = False
    Function getXpos() As Integer
        Return p_position.X
    End Function
    Public Property Chase As Boolean
        Get
            Return p_chase
        End Get
        Set(value As Boolean)
            p_chase = value
        End Set
    End Property
    Public ReadOnly Property TileoldPos As Point
        Get
            Return oldTilePos
        End Get
    End Property
    Public ReadOnly Property CanAttack As Boolean
        Get
            Return attackTime = 0
        End Get
    End Property
    Public Property Element As String
        Get
            Return _element
        End Get
        Set(value As String)
            _element = value
        End Set
    End Property
    Public Property Frozen As Boolean
        Get
            Return _freeze
        End Get
        Set(value As Boolean)
            _freeze = value
        End Set
    End Property
    Public Sub New(ByRef game As Game)
        p_game = game
        p_chase = False
        p_position = New PointF(0, 0)
        p_direction = 1
        p_state = AnimationStates.Standing
        range = New Rectangle(0, 0, 0, 0)
        p_moving = True
        p_visible = True

        REM initialize loadable properties
        drainVictim = Nothing
        _bleedTime = 0.0F
        _freezeTime = 0.0F
        _regenTime = 0.0F
        _bleeding = False
        _regen = False
        _freeze = False
        p_name = ""
        p_class = ""
        p_race = ""
        p_desc = ""
        p_str = 0
        p_dex = 0
        p_sta = 0
        p_int = 0
        p_cha = 0
        p_hitpoints = 0
        p_health = 0
        p_experience = 0
        p_level = 1
        p_alive = True
        p_dropGold1 = 0
        p_dropGold2 = 0
        p_walkSprite = Nothing
        p_walkFilename = ""
        p_walkSize = New Point(0, 0)
        p_walkColumns = 0
        p_attackSprite = Nothing
        p_attackFilename = ""
        p_attackSize = New Point(0, 0)
        p_attackColumns = 0
        p_dieSprite = Nothing
        p_dieFilename = ""
        p_dieSize = New Point(0, 0)
        p_dieColumns = 0
        talkFilname = ""
        talkSize = New Point(0, 0)
        talkColumns = 0
        talkSize = Nothing
        p_dropnum1 = 0
        p_dropnum2 = 0
        p_dropnum3 = 0
        p_dropitem1 = ""
        p_dropitem2 = ""
        p_dropitem3 = ""
        fireSprite = p_game.ReturnFireSprite(p_game, New Point(Position.X + 32, Position.Y + 32))
        iceSprite = p_game.ReturnIceSprite(p_game, New Point(Position.X + 32, Position.Y + 32))
        platformSprite = p_game.ReturnPlatformSprite(p_game, New Point(Position.X + 32, Position.Y + 32))
    End Sub
    Public ReadOnly Property getSpriteSize As Point
        Get
            Select Case AnimationState
                Case AnimationStates.Attacking
                    Return p_attackSize
                Case AnimationStates.Dying, AnimationStates.Dead
                    Return p_dieSize
                Case AnimationStates.Standing, AnimationStates.Walking
                    Return p_walkSize
                Case AnimationStates.Talking
                    Return talkSize
            End Select
        End Get
    End Property
    Public Property Bleeding As Boolean
        Get
            Return _bleeding
        End Get
        Set(value As Boolean)
            _bleeding = value
        End Set
    End Property
    Public Property Regenerating As Boolean
        Get
            Return _regen
        End Get
        Set(value As Boolean)
            _regen = value
        End Set
    End Property
    Public Property AttackFlag As Boolean
        Get
            Return p_AttackFlag
        End Get
        Set(value As Boolean)
            p_AttackFlag = value
        End Set
    End Property
    Public Property Visible As Boolean
        Get
            Return p_visible
        End Get
        Set(value As Boolean)
            p_visible = value
        End Set
    End Property
    Public Property Name() As String
        Get
            Return p_name
        End Get
        Set(ByVal value As String)
            p_name = value
        End Set
    End Property
    Public Property PlayerClass() As String
        Get
            Return p_class
        End Get
        Set(ByVal value As String)
            p_class = value
        End Set
    End Property
    Public Property Race() As String
        Get
            Return p_race
        End Get
        Set(ByVal value As String)
            p_race = value
        End Set
    End Property
    Public Property Description() As String
        Get
            Return p_desc
        End Get
        Set(ByVal value As String)
            p_desc = value
        End Set
    End Property
    Public Property STR() As Integer
        Get
            Return p_str
        End Get
        Set(ByVal value As Integer)
            p_str = value
        End Set
    End Property
    Public Property DEX() As Integer
        Get
            Return p_dex
        End Get
        Set(ByVal value As Integer)
            p_dex = value
        End Set
    End Property
    Public Property STA() As Integer
        Get
            Return p_sta
        End Get
        Set(ByVal value As Integer)
            p_sta = value
        End Set
    End Property
    Public Property INT() As Integer
        Get
            Return p_int
        End Get
        Set(ByVal value As Integer)
            p_int = value
        End Set
    End Property
    Public Property CHA() As Integer
        Get
            Return p_cha
        End Get
        Set(ByVal value As Integer)
            p_cha = value
        End Set
    End Property
    Public Property HitPoints() As Integer
        Get
            Return p_hitpoints
        End Get
        Set(ByVal value As Integer)
            p_hitpoints = value
        End Set
    End Property
    Public Property Health() As Single
        Get
            Return p_health
        End Get
        Set(ByVal value As Single)
            p_health = value
        End Set
    End Property
    Public Property DropGoldMin() As Integer
        Get
            Return p_dropGold1
        End Get
        Set(ByVal value As Integer)
            p_dropGold1 = value
        End Set
    End Property
    Public Property DropGoldMax() As Integer
        Get
            Return p_dropGold2
        End Get
        Set(ByVal value As Integer)
            p_dropGold2 = value
        End Set
    End Property
    Public Property DropNum1() As Integer
        Get
            Return p_dropnum1
        End Get
        Set(ByVal value As Integer)
            p_dropnum1 = value
        End Set
    End Property
    Public Property DropNum2() As Integer
        Get
            Return p_dropnum2
        End Get
        Set(ByVal value As Integer)
            p_dropnum2 = value
        End Set
    End Property
    Public Property DropNum3() As Integer
        Get
            Return p_dropnum3
        End Get
        Set(ByVal value As Integer)
            p_dropnum3 = value
        End Set
    End Property
    Public Property DropItem1() As String
        Get
            Return p_dropitem1
        End Get
        Set(ByVal value As String)
            p_dropitem1 = value
        End Set
    End Property
    Public Property DropItem2() As String
        Get
            Return p_dropitem2
        End Get
        Set(ByVal value As String)
            p_dropitem2 = value
        End Set
    End Property
    Public Property DropItem3() As String
        Get
            Return p_dropitem3
        End Get
        Set(ByVal value As String)
            p_dropitem3 = value
        End Set
    End Property
    Public ReadOnly Property GetSprite() As Sprite
        Get
            Select Case p_state
                Case AnimationStates.Walking
                    Return p_walkSprite
                Case AnimationStates.Attacking
                    Return p_attackSprite
                Case AnimationStates.Dying
                    Return p_dieSprite
                Case AnimationStates.Talking
                    Return talkSprite
                Case Else
                    Return p_walkSprite
            End Select
        End Get
    End Property
    Public Property Position() As PointF
        Get
            Return p_position
        End Get
        Set(ByVal value As PointF)
            p_position = value
        End Set
    End Property
    Public Property X() As Single
        Get
            Return p_position.X
        End Get
        Set(ByVal value As Single)
            p_position.X = value
        End Set
    End Property
    Public Property Y() As Single
        Get
            Return p_position.Y
        End Get
        Set(ByVal value As Single)
            p_position.Y = value
        End Set
    End Property
    Public Property Direction() As Integer
        Get
            Return p_direction
        End Get
        Set(ByVal value As Integer)
            p_direction = value
        End Set
    End Property
    Public Property AnimationState() As AnimationStates
        Get
            Return p_state
        End Get
        Set(ByVal value As AnimationStates)
            p_state = value
        End Set
    End Property
    Public Property Moving() As Boolean
        Get
            Return p_moving
        End Get
        Set(ByVal value As Boolean)
            p_moving = value
        End Set
    End Property
    Public Overridable Sub UpdateTiles()
        oldTilePos = newTilePos
        For n As Integer = oldTilePos.X - 1 To oldTilePos.X + 1
            For m As Integer = oldTilePos.Y - 1 To oldTilePos.Y + 1
                Dim p As New PointF(n, m)
                p_game.world.SetTempCollidable(p, False)
            Next
        Next
        newTilePos = GetSpriteCurrentTilePos()
        For n As Integer = newTilePos.X - 1 To newTilePos.X + 1 Step 1
            For m As Integer = newTilePos.Y - 1 To newTilePos.Y + 1 Step 1
                Dim p As New PointF(n, m)
                If Me.Alive Then
                    If New Rectangle(n * 32, m * 32, 32, 32).IntersectsWith(New Rectangle(X, Y, GetSprite.Width, GetSprite.Height)) Then
                        p_game.world.SetTempCollidable(p, True)
                    End If
                Else
                    If New Rectangle(n * 32, m * 32, 32, 32).IntersectsWith(New Rectangle(X, Y, GetSprite.Width, GetSprite.Height)) Then
                        p_game.world.SetTempCollidable(p, False)
                    End If
                End If
            Next
        Next
    End Sub
    Public Overridable Sub Wander()
        Dim oldPos As PointF = Position
        UpdateTiles()

        'If p_game.inven.Visible = True Then AnimationState = AnimationStates.Standing

        If Not Alive Then
            p_moving = False
            If AnimationState <> AnimationStates.Dying Then AnimationState = AnimationStates.Dead
        End If

        If Frozen Then Return
        If Not p_moving Then Return
        Dim steps As Integer = 2

        If AnimationState = AnimationStates.Attacking Or AnimationState = AnimationStates.Dying Or AnimationState = AnimationStates.Talking Or Chase Or p_game.inven.Visible = True Then
            steps = 0
        End If

        REM move randomly
        Select Case Direction
            Case 1
                Y -= steps
            Case 2
                X += steps * 0.7
                Y -= steps * 0.7
            Case 0
                X += steps
            Case 5
                X += steps * 0.7
                Y += steps * 0.7
            Case 4
                Y += steps
            Case 6
                X -= steps * 0.7
                Y += steps * 0.7
            Case 7
                X -= steps
            Case 3
                X -= steps * 0.7
                Y -= steps * 0.7
        End Select

        REM keep inside boundary
        If X < range.Left Then
            Direction = p_game.Random(1, 3)
            X = range.Left
        ElseIf X > range.Right - 64 Then
            Direction = p_game.Random(5, 7)
            X = range.Right - 64
        End If
        If Y < range.Top Then
            Direction = p_game.Random(3, 5)
            Y = range.Top
        ElseIf Y > range.Bottom - 64 Then
            Direction = p_game.Random(2) - 1
            If Direction = -1 Then Direction = 7
            Y = range.Bottom - 64
        End If

        REM resolve collidable tile
        Dim pos As Point = GetCurrentTilePos()
        Dim tile As Level.tilemapStruct
        tile = p_game.world.GetTile(pos.X, pos.Y)
        If (tile.collidable) Then
            Position = oldPos
            Direction = (Direction + 7) Mod 8
            If Direction < 1 Then Direction = 0 - Direction
        ElseIf tile.tempCollidableForMonsters Then
            '   Position = oldPos
            '   Direction = (Direction + 7) Mod 8
            '   If Direction < 1 Then Direction = 0 - Direction
        End If
        REM resolve "walk overs"
        For Each monster As Character In p_game.NPCs
            If monster IsNot Me Then
                If Me.GetCurrentTilePos = monster.GetCurrentTilePos Then
                    Position = oldPos
                    Direction = (Direction + 7) Mod 8
                    If Direction < 1 Then Direction = 0 - Direction
                End If
            End If
        Next
    End Sub
    Public Sub Draw()
        Draw(p_position.X, p_position.Y)
    End Sub
    Public Sub Draw(ByVal pos As PointF)
        Draw(pos.X, pos.Y)
    End Sub
    Public Sub Draw(ByVal x As Integer, ByVal y As Integer)
        Dim startFrame As Integer
        Dim endFrame As Integer
        Static frameNumber As Integer = 0

        REM handle animation bouncing
        oldState = newState
        newState = p_state
        If (oldState <> newState) AndAlso (AnimationState <> AnimationStates.Dead AndAlso AnimationState <> AnimationStates.Standing) Then
            GetSprite.CurrentFrame = p_direction * GetSprite.Columns
        End If

        'If Regenerating Then
        '   If Me Is p_game.hero Then
        '        Me.platformSprite.Draw(p_position.X - 16, p_position.Y + 20)
        '    Else
        '        Dim relativePos As New Point(Me.CenterPos.X - p_game.world.ScrollPos.X, p_position.Y - p_game.world.ScrollPos.Y)
        '        Me.platformSprite.Draw(relativePos.X - 16, relativePos.Y + 20)
        'End If
        'End If

        Select Case p_state
            Case AnimationStates.Standing
                p_walkSprite.Position = p_position
                If p_direction > -1 Then
                    startFrame = p_direction * p_walkColumns
                    endFrame = startFrame + p_walkColumns - 1
                    p_walkSprite.CurrentFrame = endFrame
                End If
                p_walkSprite.Draw(x, y)
            Case AnimationStates.Walking
                p_walkSprite.Position = p_position
                If p_direction > -1 Then
                    startFrame = p_direction * p_walkColumns
                    endFrame = startFrame + p_walkColumns - 1
                    p_walkSprite.AnimationRate = 30
                    p_walkSprite.Animate(startFrame, endFrame)
                End If
                p_walkSprite.Draw(x, y)
            Case AnimationStates.Attacking
                p_attackSprite.Position = p_position
                If p_direction > -1 Then
                    startFrame = p_direction * p_attackColumns
                    endFrame = startFrame + p_attackColumns - 1
                    p_attackSprite.AnimationRate = 30
                    p_attackSprite.Animate(startFrame, endFrame)
                End If
                p_attackSprite.Draw(x, y)
            Case AnimationStates.Talking
                talkSprite.Position = p_position
                If p_direction > -1 Then
                    startFrame = p_direction * talkColumns
                    endFrame = startFrame + talkColumns - 1
                    talkSprite.AnimationRate = 10
                    talkSprite.Animate(startFrame, endFrame)
                End If
                talkSprite.Draw(x, y)
            Case AnimationStates.Dying
                p_dieSprite.Position = p_position
                If p_direction > -1 Then
                    startFrame = p_direction * p_dieColumns
                    endFrame = startFrame + p_dieColumns - 1
                    '   p_dieSprite.AnimationRate = 100
                    p_dieSprite.AnimationRate = 30
                    p_dieSprite.Animate(startFrame, endFrame)
                End If
                p_dieSprite.Draw(x, y)
            Case AnimationStates.Dead
                p_dieSprite.Position = p_position
                If p_direction > -1 Then
                    startFrame = p_direction * p_dieColumns
                    endFrame = startFrame + p_dieColumns - 1
                    p_dieSprite.CurrentFrame = endFrame
                End If
                p_dieSprite.Draw(x, y)
        End Select
    End Sub
    Public Function Load(ByVal filename As String) As Boolean
        Try
            REM open the xml file 
            Dim doc As New XmlDocument()
            doc.Load(filename)
            Dim list As XmlNodeList = doc.GetElementsByTagName("character")
            Dim element As XmlElement = list(0)

            REM read data fields
            p_name = getElement("name", element)
            p_class = getElement("class", element)
            p_race = getElement("race", element)
            p_desc = getElement("desc", element)
            p_str = getElement("str", element)
            p_dex = getElement("dex", element)
            p_sta = getElement("sta", element)
            p_int = getElement("int", element)
            p_cha = getElement("cha", element)
            p_hitpoints = getElement("hitpoints", element)
            p_health = p_hitpoints
            p_walkFilename = getElement("anim_walk_filename", element)

            _element = getElement("element", element)

            p_walkSize.X = Convert.ToInt32( _
                getElement("anim_walk_width", element))

            p_walkSize.Y = Convert.ToInt32( _
                getElement("anim_walk_height", element))

            p_walkColumns = Convert.ToInt32( _
                getElement("anim_walk_columns", element))

            p_attackFilename = getElement( _
                "anim_attack_filename", element)

            p_attackSize.X = Convert.ToInt32( _
                getElement("anim_attack_width", element))

            p_attackSize.Y = Convert.ToInt32( _
                getElement("anim_attack_height", element))

            p_attackColumns = Convert.ToInt32( _
                getElement("anim_attack_columns", element))

            p_dieFilename = getElement( _
                "anim_die_filename", element)

            p_dieSize.X = Convert.ToInt32( _
                getElement("anim_die_width", element))

            p_dieSize.Y = Convert.ToInt32( _
                getElement("anim_die_height", element))

            p_dieColumns = Convert.ToInt32( _
                getElement("anim_die_columns", element))

            talkFilname = getElement( _
                "anim_talk_filename", element)

            talkSize.X = Convert.ToInt32( _
                getElement("anim_talk_width", element))

            talkSize.Y = Convert.ToInt32( _
                getElement("anim_talk_height", element))

            talkColumns = Convert.ToInt32( _
                getElement("anim_talk_columns", element))


            p_dropGold1 = Convert.ToInt32( _
                getElement("dropgold1", element))

            p_dropGold2 = Convert.ToInt32( _
                getElement("dropgold2", element))

            p_dropnum1 = Convert.ToInt32(getElement("drop1_num", element))
            p_dropnum2 = Convert.ToInt32(getElement("drop2_num", element))
            p_dropnum3 = Convert.ToInt32(getElement("drop3_num", element))
            p_dropitem1 = getElement("drop1_item", element)
            p_dropitem2 = getElement("drop2_item", element)
            p_dropitem3 = getElement("drop3_item", element)

        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Return False
        End Try

        REM create character sprites
        Try
            If p_walkFilename <> "" Then
                p_walkSprite = New Sprite(p_game)
                p_walkSprite.Image = LoadBitmap(p_walkFilename)
                p_walkSprite.Size = p_walkSize
                p_walkSprite.Columns = p_walkColumns
                p_walkSprite.TotalFrames = p_walkColumns * 8
            End If

            If p_attackFilename <> "" Then
                p_attackSprite = New Sprite(p_game)
                p_attackSprite.Image = LoadBitmap(p_attackFilename)
                p_attackSprite.Size = p_attackSize
                p_attackSprite.Columns = p_attackColumns
                p_attackSprite.TotalFrames = p_attackColumns * 8
            End If

            If p_dieFilename <> "" Then
                p_dieSprite = New Sprite(p_game)
                p_dieSprite.Image = LoadBitmap(p_dieFilename)
                p_dieSprite.Size = p_dieSize
                p_dieSprite.Columns = p_dieColumns
                p_dieSprite.TotalFrames = p_dieColumns * 8
            End If

            If talkFilname <> "" Then
                talkSprite = New Sprite(p_game)
                talkSprite.Image = LoadBitmap(talkFilname)
                talkSprite.Size = talkSize
                talkSprite.Columns = talkColumns
                talkSprite.TotalFrames = talkColumns * 8
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Return False
        End Try

        Return True
    End Function
    Private Function LoadBitmap(ByVal filename As String)
        Dim bmp As Bitmap
        Try
            bmp = New Bitmap(filename)
        Catch ex As Exception
            bmp = Nothing
        End Try
        Return bmp
    End Function
    Private Function getElement(ByVal field As String, ByRef element As XmlElement) As String
        Dim value As String = ""
        Try
            value = element.GetElementsByTagName(field)(0).InnerText
        Catch ex As Exception
            REM ignore error, just return empty
            Console.WriteLine(ex.Message)
        End Try
        Return value
    End Function
    Public Overridable Function GetCurrentTilePos() As Point
        Dim feet As PointF = FootPos()
        Dim tilex As Integer = feet.X / 32
        Dim tiley As Integer = feet.Y / 32
        Return New Point(tilex, tiley)
    End Function
    Public Overridable Function GetSpriteCurrentTilePos() As Point
        Return New Point(Convert.ToInt32(X / 32), Convert.ToInt32(Y / 32))
    End Function
    Public Sub SetFootPos(tileX As Integer, tileY As Integer)
        Dim feet As PointF = FootPos
        Dim newPos As PointF
        newPos.X = (32 * tileX) - 32
        newPos.Y = (32 * tileY) - 32 - 16
        Me.X = newPos.X
        Me.Y = newPos.Y
    End Sub
    Public ReadOnly Property FootPos() As PointF
        Get
            Return New Point(Me.X + 32, Me.Y + 32 + 16)
        End Get
    End Property
    Public ReadOnly Property CenterPos() As PointF
        Get
            Dim pos As PointF = Me.Position
            pos.X += Me.GetSprite.Width / 2
            pos.Y += Me.GetSprite.Height / 2
            Return pos
        End Get
    End Property
    Public Function FootDistance(ByRef other As Character) As Single
        Return p_game.Distance(Me.FootPos, other.FootPos)
    End Function
    Public Function FootDistance(ByVal pos As PointF) As Single
        Return p_game.Distance(Me.FootPos, pos)
    End Function
    Public Function CenterDistance(ByRef other As Character) As Single
        Return p_game.Distance(CenterPos, other.CenterPos)
    End Function
    Public Function CenterDistance(ByVal pos As PointF) As Single
        Return p_game.Distance(Me.CenterPos, pos)
    End Function
    Public Overrides Function ToString() As String
        Return p_name
    End Function
    Public Property Experience() As Integer
        Get
            Return p_experience
        End Get
        Set(ByVal value As Integer)
            p_experience = value
        End Set
    End Property
    Public Property Level() As Integer
        Get
            Return p_level
        End Get
        Set(ByVal value As Integer)
            p_level = value
        End Set
    End Property
    Public Property Alive() As Boolean
        Get
            Return p_alive
        End Get
        Set(ByVal value As Boolean)
            p_alive = value
        End Set
    End Property
    Public Overridable Sub StartAttackTimer()
        attackTime = attackDelay
    End Sub
    Public Overridable Sub UpdateAttackTimer()
        If attackTime > 0 Then attackTime -= 1
    End Sub
    Public Overridable Sub UpdateStats()
        If Bleeding Then
            _bleedTime += (1000 / Form1.FrameRate)
            If Me.Alive = True Then
                Me.Health -= bleedPower
                p_game.combat.CheckEnemyDeath()
            End If
        End If
        If Frozen Then
            Me.iceSprite.Animate()
            Dim relativePos As New Point(p_position.X - p_game.world.ScrollPos.X + 24, p_position.Y - p_game.world.ScrollPos.Y)
            Me.iceSprite.Draw(relativePos.X, relativePos.Y)
        End If
        If Regenerating Then
            _regenTime += (1000 / Form1.FrameRate)
            If Me.drainVictim.Alive And (Me.Health < Me.HitPoints) Then
                Me.Health += CSng(regenPower * 0.05)
                Me.drainVictim.Health -= (regenPower * 0.5)
                p_game.combat.CheckPlayerDeath()
            Else
                If Me.Health > Me.HitPoints Then Me.Health = Me.HitPoints
            End If
            Dim relativePos As New Point(p_position.X - p_game.world.ScrollPos.X, p_position.Y - p_game.world.ScrollPos.Y)
        End If
        If _bleedTime >= 1 Then
            _bleedTime = 0.0F
            Bleeding = False
        End If
        If _regenTime >= 1 Then
            _regenTime = 0.0F
            Regenerating = False
            Me.drainVictim = Nothing
        End If
    End Sub
    Public Sub KillInstantly()
        Alive = False
        Health = 0
        AnimationState = Character.AnimationStates.Dead
    End Sub
    Public Sub Kill()
        Alive = False
        Health = 0
        AnimationState = Character.AnimationStates.Dying
    End Sub
End Class