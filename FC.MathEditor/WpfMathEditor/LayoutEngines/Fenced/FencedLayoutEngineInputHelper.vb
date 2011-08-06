Public Class FencedLayoutEngineInputHelper : Inherits InputHelper

    Public Sub New(This As FencedLayoutEngine)
        MyBase.New(This)
    End Sub

    Protected Shadows ReadOnly Property This() As FencedLayoutEngine
        Get
            Return MyBase.This
        End Get
    End Property

    Public Overrides Function ProcessChar_Internal(InputChar As Integer) As Boolean

    End Function

End Class
