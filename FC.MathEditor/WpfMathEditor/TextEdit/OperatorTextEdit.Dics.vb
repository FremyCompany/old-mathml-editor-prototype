Partial Public Class OperatorTextEdit
    '++
    '++ DICS
    '++

    Public Shared ReadOnly OperatorDic_fence As New Dictionary(Of String, Boolean) From {
        {"(", True},
        {")", True}
    }

    Public Shared ReadOnly OperatorDic_separator As New Dictionary(Of String, Boolean) From {
        {",", True},
        {"|", True}
    }

    Public Shared ReadOnly OperatorDic_accent As New Dictionary(Of String, Boolean) From {
        {"´", True},
        {"`", True},
        {"̇", True}
    }

    Public Shared ReadOnly OperatorDic_largeop As New Dictionary(Of String, Boolean) From {
        {"∑", True}
    }

    Public Shared ReadOnly OperatorDic_stretchy As New Dictionary(Of String, Boolean) From {
        {"(", True},
        {")", True},
        {"|", True},
        {"∑", True}
    }

    Public Shared ReadOnly OperatorDic_symmetric As New Dictionary(Of String, Boolean) From {
        {"(", False},
        {")", False}
    }

End Class