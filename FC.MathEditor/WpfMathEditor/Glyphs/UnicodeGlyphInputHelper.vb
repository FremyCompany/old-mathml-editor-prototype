Public Class UnicodeGlyphInputHelper : Inherits InputHelper

    Public Sub New(This As UnicodeGlyph)
        MyBase.New(This)
    End Sub

    Public Overrides Function ProcessChar_Internal(InputChar As Integer) As Boolean
        Return False
    End Function

End Class
