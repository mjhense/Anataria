<Serializable>
    Public Class tilemapStruct
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
    Public animation As Sprite
    Public tileX As Integer
    Public tileY As Integer
    Public ReadOnly Property Bounds As Rectangle
        Get
            Return New Rectangle(tileX * 32, tileY * 32, 32, 32)
        End Get
    End Property
    Public Sub New()
    End Sub
End Class
