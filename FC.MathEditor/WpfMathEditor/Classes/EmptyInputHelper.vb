Public Class EmptyInputHelper : Inherits InputHelper

    Public Sub New(This As MathElement)
        MyBase.New(This)
    End Sub

    Public Overrides Function ProcessChar_Internal(InputChar As Integer) As Boolean
        Return False
    End Function

End Class
