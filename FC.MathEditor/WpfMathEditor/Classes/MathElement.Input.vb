Partial Public Class MathElement

    Private _Input As InputHelper
    Public Property Input As InputHelper
        Get
            If Me.ParentDocument Is Nothing Then Return Nothing
            Return _Input
        End Get
        Set(ByVal value As InputHelper)
            _Input = value
        End Set
    End Property

    Private _Selection As SelectionHelper
    Public Overridable ReadOnly Property Selection As SelectionHelper
        Get
            If Me.ParentDocument Is Nothing Then Return Nothing
            If _Selection Is Nothing Then _Selection = ParentDocument.Selection
            Return _Selection
        End Get
    End Property

    Public Event GotFocus As EventHandler
    Public Event LostFocus As EventHandler

End Class
