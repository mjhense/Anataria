Public Class Quest
    Private p_title As String
    Private p_summary As String
    Private p_desc As String
    Private p_RequiredItemFlag As Boolean
    Private p_RequiredItemCount As Integer
    Private p_RequiredItem As String
    Private p_RequiredLocFlag As Boolean
    Private p_RequiredLocX As Integer
    Private p_RequiredLocY As Integer
    Private p_RewardXP As Integer
    Private p_RewardGold As Integer
    Private p_RewardItem As String
    Private p_RequiredNPCFlag As Boolean
    Private p_RequiredNPC As String
    Private p_RequiredKillsCount As Integer
    Private p_RequiredKillsNPC As String
    Private p_RequiredKillsFlag As Boolean
    Private p_RequiredPortalFile As String
    Public Sub New()
        p_title = "new quest"
        p_summary = ""
        p_desc = ""
        p_RequiredItemFlag = False
        p_RequiredItemCount = 0
        p_RequiredItem = ""
        p_RequiredLocFlag = False
        p_RequiredLocX = 0
        p_RequiredLocY = 0
        p_RequiredKillsFlag = False
        p_RequiredNPCFlag = False
        p_RewardXP = 0
        p_RewardGold = 0
        p_RewardItem = ""
        p_RequiredPortalFile = ""
    End Sub
    Public Overrides Function ToString() As String
        Return p_title
    End Function
    Public Property Title() As String
        Get
            Return p_title
        End Get
        Set(ByVal value As String)
            p_title = value
        End Set
    End Property
    Public Property Summary() As String
        Get
            Return p_summary
        End Get
        Set(ByVal value As String)
            p_summary = value
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
    Public Property RequiredItemFlag() As Boolean
        Get
            Return p_RequiredItemFlag
        End Get
        Set(ByVal value As Boolean)
            p_RequiredItemFlag = value
        End Set
    End Property
    Public Property RequiredItemCount() As Integer
        Get
            Return p_RequiredItemCount
        End Get
        Set(ByVal value As Integer)
            p_RequiredItemCount = value
        End Set
    End Property
    Public Property RequiredItem() As String
        Get
            Return p_RequiredItem
        End Get
        Set(ByVal value As String)
            p_RequiredItem = value
        End Set
    End Property
    Public Property RequiredLocFlag() As Boolean
        Get
            Return p_RequiredLocFlag
        End Get
        Set(ByVal value As Boolean)
            p_RequiredLocFlag = value
        End Set
    End Property
    Public Property RequiredLocX() As Integer
        Get
            Return p_RequiredLocX
        End Get
        Set(ByVal value As Integer)
            p_RequiredLocX = value
        End Set
    End Property
    Public Property RequiredLocY() As Integer
        Get
            Return p_RequiredLocY
        End Get
        Set(ByVal value As Integer)
            p_RequiredLocY = value
        End Set
    End Property
    Public Property RewardXP() As Integer
        Get
            Return p_RewardXP
        End Get
        Set(ByVal value As Integer)
            p_RewardXP = value
        End Set
    End Property
    Public Property RewardGold() As Integer
        Get
            Return p_RewardGold
        End Get
        Set(ByVal value As Integer)
            p_RewardGold = value
        End Set
    End Property
    Public Property RewardItem() As String
        Get
            Return p_RewardItem
        End Get
        Set(ByVal value As String)
            p_RewardItem = value
        End Set
    End Property
    Public Property RequiredNPC() As String
        Get
            Return p_RequiredNPC
        End Get
        Set(value As String)
            p_RequiredNPC = value
        End Set
    End Property
    Public Property RequiredKillNPC() As String
        Get
            Return p_RequiredKillsNPC
        End Get
        Set(value As String)
            p_RequiredKillsNPC = value
        End Set

    End Property
    Public Property RequiredKillCount() As Integer
        Get
            Return p_RequiredKillsCount
        End Get
        Set(value As Integer)
            p_RequiredKillsCount = value
        End Set
    End Property
    Public Property RequiredNPCFlag() As Boolean
        Get
            Return p_RequiredNPCFlag
        End Get
        Set(value As Boolean)
            p_RequiredNPCFlag = value
        End Set
    End Property
    Public Property RequiredKillNPCFlag() As Boolean
        Get
            Return p_RequiredKillsFlag
        End Get
        Set(value As Boolean)
            p_RequiredKillsFlag = value
        End Set
    End Property
    Public Property RequiredPortalFile As String
        Get
            Return p_RequiredPortalFile
        End Get
        Set(value As String)
            p_RequiredPortalFile = value
        End Set
    End Property
End Class