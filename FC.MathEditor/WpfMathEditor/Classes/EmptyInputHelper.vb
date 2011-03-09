Public Class EmptyInputHelper : Inherits InputHelper

    Public Sub New(ByVal This As MathElement)
        MyBase.New(This)
    End Sub

    Public Overrides Function ProcessChar_Internal(ByVal InputChar As Integer) As Boolean
        Return False
    End Function

End Class
