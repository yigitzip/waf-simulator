package com.example.backend.controller;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/comments")
@CrossOrigin(origins = "http://localhost:4200")

public class CommentController {

    @PostMapping
    public ResponseEntity<String> addComment(@RequestBody CommentRequest request) {

        return ResponseEntity.ok(
                "Comment Received: " + request.comment()
        );
    }

    public record CommentRequest(String comment) {

    }
}
