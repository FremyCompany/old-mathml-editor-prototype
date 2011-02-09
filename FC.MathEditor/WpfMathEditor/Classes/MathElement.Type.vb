Partial Public Class MathElement
    Public Enum Type
        LayoutEngine = 2 ^ 0
        Formatter = 2 ^ 1
        TextEdit = (2 ^ 0) + (2 ^ 2)
        Glyph = 2 ^ 3
    End Enum
End Class