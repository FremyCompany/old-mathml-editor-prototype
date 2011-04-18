Module QualityControl

#If DEBUG Then

    Public Sub AssertFalse(ByVal X As Boolean)
        If X Then Throw New InvalidOperationException("An invalid operation has led to a corrupted state.")
    End Sub

    Public Sub AssertTrue(ByVal X As Boolean)
        If Not X Then Throw New InvalidOperationException("An invalid operation has led to a corrupted state.")
    End Sub

    Public Sub AssertNull(ByVal X As Object)
        If X IsNot Nothing Then Throw New InvalidOperationException("An invalid operation has led to a corrupted state.")
    End Sub

    Public Sub AssertNotNull(ByVal X As Object)
        If X Is Nothing Then Throw New InvalidOperationException("An invalid operation has led to a corrupted state.")
    End Sub

    Public Sub AssertEquals(ByVal X As Object, ByVal Y As Object)
        If X Is Y Then Throw New InvalidOperationException("An invalid operation has led to a corrupted state.")
    End Sub

#End If

End Module
