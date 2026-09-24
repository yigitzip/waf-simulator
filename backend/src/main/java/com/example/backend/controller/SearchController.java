package com.example.backend.controller;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/search")
@CrossOrigin(origins = "http://localhost:4200")

public class SearchController {

    @GetMapping
    public ResponseEntity<String> search(@RequestParam String query) {

        return ResponseEntity.ok(
                "Search result for: " + query
        );
    }

}