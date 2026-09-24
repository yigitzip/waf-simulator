package com.example.backend.controller;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/commands")
@CrossOrigin(origins = "http://localhost:4200")
public class CommandController {

    @PostMapping
    public ResponseEntity<String> executeCommand(@RequestBody CommandRequest request) {

        return ResponseEntity.ok(
                "Command received: " + request.command()
        );
    }

    public record CommandRequest(String command) {
    }
}