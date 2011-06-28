Partial Public Class MathElement
    Public Enum Type
        LayoutEngine = 2 ^ 0
        Formatter = 2 ^ 1
        TextEdit = (2 ^ 0) + (2 ^ 2)
        Glyph = 2 ^ 3
    End Enum

    Public ReadOnly Property ElementType As Type
        Get
            Return Children.ElementType
        End Get
    End Property

    ''' <summary>
    ''' Returns True if this element is a formatter element (a chosen amount of children, displayed in a formatted manner).
    ''' </summary>
    Public ReadOnly Property IsFormatter() As Boolean
        Get
            Return (Children.ElementType And Type.Formatter) = Type.Formatter
        End Get
    End Property

    ''' <summary>
    ''' Returns True if this element is a layout engine (as many child as you want).
    ''' </summary>
    Public ReadOnly Property IsLayoutEngine As Boolean
        Get
            Return (Children.ElementType And Type.LayoutEngine) = Type.LayoutEngine
        End Get
    End Property

    ''' <summary>
    ''' Returns True if this element is a text editor (can only contains glyph elements).
    ''' </summary>
    Public ReadOnly Property IsTextEdit As Boolean
        Get
            Return (Children.ElementType And Type.TextEdit) = Type.TextEdit
        End Get
    End Property

    ''' <summary>
    ''' Returns True if this element is a glyph (doesn't have any child, has a fixed layout)
    ''' </summary>
    Public ReadOnly Property IsGlyph As Boolean
        Get
            Return (Children.ElementType And Type.Glyph) = Type.Glyph
        End Get
    End Property


End Class