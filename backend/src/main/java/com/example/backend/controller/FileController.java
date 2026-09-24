package com.example.backend.controller;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/files")
@CrossOrigin(origins = "http://localhost:4200")
public class FileController {

    @GetMapping
    public ResponseEntity<String> getFile(@RequestParam String path) {

        return ResponseEntity.ok(
                "Requested file: " + path
        );
    }
}