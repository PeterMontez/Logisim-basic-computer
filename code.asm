    mov     $0, 16
    mov     $1, 8
    lsh     $0, $1
    mov     $1, 63
    mov     $2, 128
    lsh     $2, $1
    mov     $3, 0
    mov     $4, 1
    mov     [$0], $2
    add     $0, $1
    mov     [$0], $4
end:
    jump    end
