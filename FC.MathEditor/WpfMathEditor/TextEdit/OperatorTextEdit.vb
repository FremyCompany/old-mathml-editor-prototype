Public Class OperatorTextEdit : Inherits TextEdit

    Public Sub New()
        ' Do nothing
    End Sub

    Public Sub New(Children As IEnumerable(Of MathElement))
        Call Me.New()
        For Each C In Children
            Me.Children.Add(C)
        Next
    End Sub

    Public Overrides Function TryGetDefaultAttribute(AttributeName As String, ByRef Result As String) As Boolean
        Select Case AttributeName
            Case "mathvariant"
                Result = "serif" : Return True
            Case Else
                Return MyBase.TryGetDefaultAttribute(AttributeName, Result)
        End Select
    End Function

    Public Overrides Function Clone_Internal() As MathElement
        Return New OperatorTextEdit()
    End Function

    Public Overrides ReadOnly Property ElementName As String
        Get
            Return "mo"
        End Get
    End Property

    Public Overrides Function IsAccepted(C As Integer, IsFirst As Boolean) As Boolean
        Return IsFirst
    End Function

    Public Overrides ReadOnly Property EatInputByDefault As Boolean
        Get
            Return False
        End Get
    End Property

End Class
